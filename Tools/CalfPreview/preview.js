import {adaptCalf} from './CalfModel.js';
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
let mixer,model,head,action,clips=[],paused=false;const clock=new THREE.Clock();let addedTriangles=0;
function setClip(name){if(!mixer)return;const clip=clips.find(c=>c.name===name);if(!clip)return;action?.fadeOut(.2);action=mixer.clipAction(clip);action.reset().fadeIn(.2).play();document.querySelector('#status').textContent=`${name} animation • 13 source clips\nOriginal rig retained • Preview adaptation`;
 document.querySelector('#status').style.whiteSpace='pre-line';}
try{
 const gltf=await new GLTFLoader().loadAsync('./base.gltf');model=gltf.scene;clips=gltf.animations;scene.add(model);model.updateMatrixWorld(true);
 ({head,addedTriangles}=adaptCalf(model));
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
