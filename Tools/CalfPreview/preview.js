import * as THREE from 'three';
import {GLTFLoader} from 'three/addons/loaders/GLTFLoader.js';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';

const renderer=new THREE.WebGLRenderer({canvas:document.querySelector('#view'),antialias:true,preserveDrawingBuffer:true});
renderer.setPixelRatio(Math.min(devicePixelRatio,2));renderer.shadowMap.enabled=true;renderer.shadowMap.type=THREE.PCFSoftShadowMap;
renderer.outputColorSpace=THREE.SRGBColorSpace;renderer.toneMapping=THREE.ACESFilmicToneMapping;renderer.toneMappingExposure=1.05;
const scene=new THREE.Scene();scene.background=new THREE.Color('#202b2b');scene.fog=new THREE.Fog('#202b2b',9,20);
const camera=new THREE.PerspectiveCamera(36,innerWidth/innerHeight,.01,100);camera.position.set(3,1.8,3.6);
const controls=new OrbitControls(camera,renderer.domElement);controls.target.set(0,.65,.22);controls.enableDamping=true;controls.maxDistance=7;controls.minDistance=2;controls.maxPolarAngle=Math.PI*.49;
scene.add(new THREE.HemisphereLight('#fff1d3','#77948d',2.5));
const key=new THREE.DirectionalLight('#ffe8ca',3);key.position.set(3,6,4);key.castShadow=true;key.shadow.mapSize.set(2048,2048);key.shadow.camera.left=-4;key.shadow.camera.right=4;key.shadow.camera.top=4;key.shadow.camera.bottom=-4;key.shadow.bias=-.001;scene.add(key);
const fill=new THREE.DirectionalLight('#a7e6dc',2);fill.position.set(-4,3,-3);scene.add(fill);
const floor=new THREE.Mesh(new THREE.CircleGeometry(12,96),new THREE.MeshStandardMaterial({color:'#344540',roughness:.85}));floor.rotation.x=-Math.PI/2;floor.receiveShadow=true;floor.position.y=-.045;scene.add(floor);
const plinth=new THREE.Mesh(new THREE.CylinderGeometry(1.7,1.74,.08,80),new THREE.MeshStandardMaterial({color:'#596258',roughness:.8}));plinth.position.y=-.06;plinth.receiveShadow=true;scene.add(plinth);
const coatColors=['#a65d32','#b96e3b','#bd7a44','#c5844a','#ab6534'];
const coatMaterials=coatColors.map(color=>new THREE.MeshStandardMaterial({color,roughness:.96,flatShading:false}));
const hornMaterial=new THREE.MeshStandardMaterial({color:'#ead7ae',roughness:.8,flatShading:true});
let mixer,model,head,action,clips=[],paused=false;const clock=new THREE.Clock();let addedTriangles=0;
function lockGeometry(){const points=[],indices=[];const levels=[[.55,.06],[.44,.37],[.22,.58],[-.06,.56],[-.35,.36],[-.60,.02]],n=10;for(const[y,r]of levels)for(let i=0;i<n;i++){const a=i/n*Math.PI*2;points.push(Math.cos(a)*r,y,Math.sin(a)*r);}for(let j=0;j<5;j++)for(let i=0;i<n;i++){let a=j*n+i,b=j*n+(i+1)%n,c=a+n,d=b+n;indices.push(a,b,c,b,d,c);}const g=new THREE.BufferGeometry();g.setAttribute('position',new THREE.Float32BufferAttribute(points,3));g.setIndex(indices);g.computeVertexNormals();return g;}
const lock=lockGeometry();
function attach(mesh,bone,position,scale,rotation=0){mesh.position.set(...position);mesh.scale.set(...scale);mesh.rotation.z=rotation;mesh.updateMatrix();const bind=new THREE.Matrix4().copy(bone.matrixWorld).invert().multiply(mesh.matrix);bind.decompose(mesh.position,mesh.quaternion,mesh.scale);bone.add(mesh);mesh.castShadow=true;mesh.receiveShadow=true;addedTriangles+=(mesh.geometry.index?.count||mesh.geometry.attributes.position.count)/3;}
function tuft(position,scale,index,bone,rotation=0){attach(new THREE.Mesh(lock,coatMaterials[index%coatMaterials.length]),bone,position,scale,rotation);}
function setClip(name){if(!mixer)return;const clip=clips.find(c=>c.name===name);if(!clip)return;action?.fadeOut(.2);action=mixer.clipAction(clip);action.reset().fadeIn(.2).play();document.querySelector('#status').textContent=`${name} animation • 13 source clips\nOriginal rig retained • Preview adaptation`;
 document.querySelector('#status').style.whiteSpace='pre-line';}
try{
 const gltf=await new GLTFLoader().loadAsync('./base.gltf');model=gltf.scene;clips=gltf.animations;scene.add(model);model.updateMatrixWorld(true);
 head=model.getObjectByName('Head');const torso=model.getObjectByName('Torso3'),neck=model.getObjectByName('Neck1');
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
 mixer=new THREE.AnimationMixer(model);setClip(document.querySelector('#clip').value);
 document.body.dataset.sourceClips=clips.length;document.body.dataset.addedTriangles=addedTriangles;
}catch(e){document.querySelector('#status').id='error';document.querySelector('#error').textContent=e.message;console.error(e);}
document.querySelector('#clip').onchange=e=>setClip(e.target.value);
document.querySelector('#pause').onclick=e=>{paused=!paused;e.target.textContent=paused?'Play':'Pause';};
function view(x,y,z){camera.position.set(x,y,z);controls.target.set(0,.65,.2);controls.update();}
document.querySelector('#front').onclick=()=>view(0,1.35,4.4);
document.querySelector('#side').onclick=()=>view(4.4,1.5,.1);
document.querySelector('#threequarter').onclick=()=>view(3,1.8,3.6);
function resize(){renderer.setSize(innerWidth,innerHeight);camera.aspect=innerWidth/innerHeight;camera.updateProjectionMatrix();}addEventListener('resize',resize);resize();
renderer.setAnimationLoop(()=>{const dt=Math.min(clock.getDelta(),.05);if(!paused)mixer?.update(dt);if(head)head.scale.set(1.17,1.17,1.17);controls.update();renderer.render(scene,camera);});
