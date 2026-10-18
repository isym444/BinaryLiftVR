import * as THREE from 'three';

// Shared approved adaptation: browser review and Unity export use identical geometry.
export function adaptCalf(model) {
const coatColors=['#a65d32','#b96e3b','#bd7a44','#c5844a','#ab6534'];
const coatMaterials=coatColors.map(color=>new THREE.MeshStandardMaterial({color,roughness:.96,flatShading:false}));
const hornMaterial=new THREE.MeshStandardMaterial({color:'#ead7ae',roughness:.8,flatShading:true});
let addedTriangles=0;
function lockGeometry(){const points=[],indices=[];const levels=[[.55,.06],[.44,.37],[.22,.58],[-.06,.56],[-.35,.36],[-.60,.02]],n=10;for(const[y,r]of levels)for(let i=0;i<n;i++){const a=i/n*Math.PI*2;points.push(Math.cos(a)*r,y,Math.sin(a)*r);}for(let j=0;j<5;j++)for(let i=0;i<n;i++){let a=j*n+i,b=j*n+(i+1)%n,c=a+n,d=b+n;indices.push(a,b,c,b,d,c);}const g=new THREE.BufferGeometry();g.setAttribute('position',new THREE.Float32BufferAttribute(points,3));g.setIndex(indices);g.computeVertexNormals();return g;}
const lock=lockGeometry();
function attach(mesh,bone,position,scale,rotation=0){mesh.position.set(...position);mesh.scale.set(...scale);mesh.rotation.z=rotation;mesh.updateMatrix();const bind=new THREE.Matrix4().copy(bone.matrixWorld).invert().multiply(mesh.matrix);bind.decompose(mesh.position,mesh.quaternion,mesh.scale);bone.add(mesh);mesh.castShadow=true;mesh.receiveShadow=true;addedTriangles+=(mesh.geometry.index?.count||mesh.geometry.attributes.position.count)/3;}
function tuft(position,scale,index,bone,rotation=0){attach(new THREE.Mesh(lock,coatMaterials[index%coatMaterials.length]),bone,position,scale,rotation);}
 const head=model.getObjectByName('Head');const torso=model.getObjectByName('Torso3'),neck=model.getObjectByName('Neck1');
 model.traverse(o=>{if(!o.isMesh)return;o.castShadow=true;o.receiveShadow=true;const m=o.material;o.material=m.clone();if(m.name==='Horns'){o.visible=false;return;}const colours={Main:'#b56b35',Main_Light:'#ce955b',Hooves:'#423a32',Muzzle:'#795241',Eye_Black:'#171b1b',Eye_White:'#fff8e7'};if(colours[m.name])o.material.color.set(colours[m.name]);o.material.roughness=.93;const positions=o.geometry.attributes.position;for(let i=0;i<positions.count;i++){const z=positions.getZ(i);if(z>3.95){positions.setZ(i,3.95+(z-3.95)*.72);if(z>4.7)positions.setX(i,positions.getX(i)*1.18);}}positions.needsUpdate=true;o.geometry.computeVertexNormals();});
 // Bind authored coat locks in the source rest pose, so they follow the existing bones.
 attach(new THREE.Mesh(new THREE.SphereGeometry(1,16,12),coatMaterials[1]),head,[0,4.3,4.13],[.80,.56,.65]);
 for(let i=0;i<7;i++)tuft([(i-3)*.21,4.29+(i%2)*.10,4.61],[.45,.82+(i%3)*.08,.39],i,head,(i-3)*-.07);
 for(const side of [-1,1]){
  for(let i=0;i<3;i++)tuft([side*(.70+i*.11),4.13-i*.07,4.0],[.34,.48,.40],i+2,head,side*.6);
  for(let i=0;i<2;i++)tuft([side*.75,3.7-i*.23,3.75],[.53,.88,.63],i,head,side*-.15);
  const curve=new THREE.QuadraticBezierCurve3(new THREE.Vector3(side*.72,4.48,4.02),new THREE.Vector3(side*1.04,4.52,4.00),new THREE.Vector3(side*1.14,4.77,4.06));
  const geom=new THREE.TubeGeometry(curve,7,.105,7,false);const pos=geom.attributes.position;
  for(let j=0;j<=7;j++){const centre=curve.getPointAt(j/7),factor=1-j/7*.83;for(let k=0;k<=7;k++){const idx=j*8+k;const v=new THREE.Vector3().fromBufferAttribute(pos,idx).sub(centre).multiplyScalar(factor).add(centre);pos.setXYZ(idx,v.x,v.y,v.z);}}geom.computeVertexNormals();
  attach(new THREE.Mesh(geom,hornMaterial),head,[0,0,0],[1,1,1]);
  for(let i=0;i<9;i++)tuft([side*1.11,3.10+(i%3)*.09,-2.1+i*.48],[.50,1.55+(i%2)*.18,.75],i,torso,side*.12);
  for(let i=0;i<4;i++)tuft([side*(.25+i*.2),2.9,2.17],[.42,1.1,.52],i,neck,side*-.12);
 }
 for(let i=0;i<6;i++)tuft([0,4.29,-2+i*.76],[1.15,.48,.77],i,torso);
 // Compact silhouette for this proportion study; source animation data is unchanged.
 model.scale.set(.29,.215,.20);model.position.z=-.18;model.position.y=.015;
 return {head, addedTriangles};
}
