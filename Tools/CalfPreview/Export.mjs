// Run with Node after Prepare.ps1. No model data or library is fetched by this exporter.
import fs from 'node:fs';
import path from 'node:path';
import {pathToFileURL, fileURLToPath} from 'node:url';
import crypto from 'node:crypto';
const repo=path.resolve(path.dirname(fileURLToPath(import.meta.url)),'../..');
const work=path.join(repo,'Logs/AssetReview/CalfPreview');
const source=path.join(repo,'SourceArt/HighlandCalf/Bull.gltf');
if(crypto.createHash('sha256').update(fs.readFileSync(source)).digest('hex').toUpperCase()!=='535DA2992EB125D517725159A2EDDEB520B72595BCB3021985FC43D2C84BFF76')throw Error('Unverified base model');
fs.mkdirSync(path.join(work,'node_modules/three'),{recursive:true});
fs.writeFileSync(path.join(work,'package.json'),'{"type":"module"}');
fs.writeFileSync(path.join(work,'node_modules/three/package.json'),'{"type":"module","exports":"./three.module.js"}');
fs.copyFileSync(path.join(work,'vendor/three.module.js'),path.join(work,'node_modules/three/three.module.js'));
fs.copyFileSync(path.join(repo,'Tools/CalfPreview/CalfModel.js'),path.join(work,'CalfModel.js'));
const THREE=await import(pathToFileURL(path.join(work,'node_modules/three/three.module.js')));
const {GLTFLoader}=await import(pathToFileURL(path.join(work,'vendor/addons/loaders/GLTFLoader.js')));
const {adaptCalf}=await import(pathToFileURL(path.join(work,'CalfModel.js')));
globalThis.ProgressEvent=class {constructor(type,info){this.type=type;Object.assign(this,info);}};
const gltf=await new GLTFLoader().parseAsync(fs.readFileSync(source,'utf8'),'');
const model=gltf.scene;model.updateMatrixWorld(true);adaptCalf(model);
// Bake in source rest space. Unity applies the approved proportions above the rig.
model.scale.set(1,1,1);model.position.set(0,0,0);model.updateMatrixWorld(true);
const nodes=[];model.traverse(o=>{if(!o.isMesh)nodes.push(o);});
const index=new Map(nodes.map((o,i)=>[o,i]));
const bones=[];model.traverse(o=>{if(o.isBone)bones.push(o);});
const boneIndex=new Map(bones.map((o,i)=>[o,i]));
const materials=[],parts=[];
const positions=[],normals=[],joints=[],weights=[];
model.traverse(o=>{
 if(!o.isMesh||!o.visible)return;
 const g=o.geometry,m=o.material;let material=materials.findIndex(x=>x.uuid===m.uuid);
 if(material<0){material=materials.length;materials.push({uuid:m.uuid,name:m.name||`Coat${material}`,color:m.color.toArray()});}
 const offset=positions.length/3,nmat=new THREE.Matrix3().getNormalMatrix(o.matrixWorld);
 let attached=o.parent;while(attached&&!attached.isBone)attached=attached.parent;
 for(let i=0;i<g.attributes.position.count;i++){
  positions.push(...new THREE.Vector3().fromBufferAttribute(g.attributes.position,i).applyMatrix4(o.matrixWorld).toArray());
  normals.push(...new THREE.Vector3().fromBufferAttribute(g.attributes.normal,i).applyNormalMatrix(nmat).toArray());
  if(o.isSkinnedMesh){for(let k=0;k<4;k++){joints.push(boneIndex.get(o.skeleton.bones[g.attributes.skinIndex.array[i*4+k]]));weights.push(g.attributes.skinWeight.array[i*4+k]);}}
  else {if(!attached)throw Error('Unbound coat');joints.push(boneIndex.get(attached),0,0,0);weights.push(1,0,0,0);}
 }
 let part=parts.find(p=>p.material===material);if(!part){part={material,triangles:[]};parts.push(part);}
 const indices=g.index?Array.from(g.index.array):Array.from({length:g.attributes.position.count},(_,i)=>i);
 part.triangles.push(...indices.map(i=>i+offset));
});
const clips=gltf.animations.map(c=>({name:c.name,duration:c.duration,tracks:c.tracks.map(t=>{
 const binding=THREE.PropertyBinding.parseTrackName(t.name),target=THREE.PropertyBinding.findNode(model,binding.nodeName);
 if(!index.has(target))throw Error(`Missing animation target ${t.name}`);
 return {node:index.get(target),property:binding.propertyName,size:t.getValueSize(),times:Array.from(t.times),values:Array.from(t.values)};
})}));
const data={version:1,nodes:nodes.map(o=>({name:o.name||'Rig',parent:index.get(o.parent)??-1,position:o.position.toArray(),rotation:o.quaternion.toArray(),scale:o.scale.toArray()})),bones:bones.map(b=>index.get(b)),head:index.get(model.getObjectByName('Head')),positions,normals,joints,weights,materials:materials.map(({uuid,...m})=>m),parts,clips};
// Fail loudly rather than allowing malformed data into the Unity importer.
if(joints.some(x=>!Number.isInteger(x))||weights.some(x=>!Number.isFinite(x)))throw Error('Invalid skin');
fs.writeFileSync(path.join(repo,'Assets/Resources/Approved/HighlandCalf.calf'),JSON.stringify(data));
console.log(JSON.stringify({vertices:positions.length/3,triangles:parts.reduce((n,p)=>n+p.triangles.length/3,0),bones:bones.length,materials:materials.length,clips:clips.map(c=>c.name)}));
