"""Draw a design storyboard using geometric shapes; no media assets are used."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

OUT = Path(__file__).parent
S = 2
im = Image.new('RGB', (1440*S, 1170*S), '#101b25')
d = ImageDraw.Draw(im)
FONT = 'C:/Windows/Fonts/segoeui.ttf'
BOLD = 'C:/Windows/Fonts/seguisb.ttf'
def font(n, bold=False): return ImageFont.truetype(BOLD if bold else FONT, n*S)
def poly(p,c): d.polygon([(int(x*S),int(y*S)) for x,y in p],fill=c)
def rect(box,c): d.rectangle(tuple(int(x*S) for x in box), fill=c)
def line(p,c,w=2): d.line([(int(x*S),int(y*S)) for x,y in p],fill=c,width=int(w*S))
def ellipse(box,c): d.ellipse(tuple(int(x*S) for x in box),fill=c)
def text(x,y,t,n=18,c='#e9ece9',bold=False): d.text((int(x*S),int(y*S)),t,font=font(n,bold),fill=c)
def tag(x,y,t,w=210):
    rect((x,y,x+w,y+33),'#112632');text(x+12,y+5,t,16,'#9de1df',True)
def panel(x,y,num,title,sub):
    rect((x,y,x+680,y+405),'#192d39')
    text(x,y-42,num+'  '+title,23,'#f4eee4',True)
    text(x,y+415,sub,17,'#bacbd0')
def cone(x,y,k=1):
    poly([(x,y-26*k),(x-12*k,y),(x+12*k,y)],'#fc8b48')
    poly([(x-5*k,y-15*k),(x+5*k,y-15*k),(x+8*k,y-9*k),(x-8*k,y-9*k)],'#f0e5cd')
    rect((x-16*k,y,x+16*k,y+4*k),'#ee7140')
def monkey(x,y,k=1):
    ellipse((x-23*k,y-11*k,x+23*k,y+38*k),'#ad805d')
    ellipse((x-23*k,y-39*k,x+23*k,y+3*k),'#93684e')
    ellipse((x-33*k,y-28*k,x-17*k,y-10*k),'#c18c65')
    ellipse((x+17*k,y-28*k,x+33*k,y-10*k),'#c18c65')
    ellipse((x-15*k,y-26*k,x+15*k,y-3*k),'#d8af7c')
    for a in [-8,8]: ellipse((x+(a-2)*k,y-20*k,x+(a+2)*k,y-16*k),'#162731')
    line([(x-12*k,y+29*k),(x-30*k,y+57*k)],'#ad805d',10*k)
    line([(x+10*k,y+29*k),(x+36*k,y+39*k)],'#ad805d',10*k)
    line([(x+19*k,y+8*k),(x+46*k,y-7*k)],'#ad805d',9*k)
    line([(x-20*k,y+20*k),(x-43*k,y+16*k),(x-48*k,y-4*k)],'#93684e',6*k)
    poly([(x+32*k,y-18*k),(x+57*k,y-13*k),(x+51*k,y+11*k),(x+28*k,y+6*k)],'#fd8747')

text(40,24,'CONE COURIER / REVISED EXPERIENCE',32,'#f5efe4',True)
text(40,70,'DESIGN STORYBOARD  •  Proposed layout and interaction, not an in-game screenshot',19,'#a7bdc6')
text(40,100,'Glasgow-inspired campus  /  First-person VR  /  Code-drawn geometry only',17,'#81cbc9')

# Classroom: furniture, readable exit, daylight and a calm exploration space.
x,y=30,190
panel(x,y,'01','CLASSROOM → COURTYARD','Explore first. The theft starts only after you leave and approach the courtyard.')
poly([(x,y),(x+680,y),(x+680,y+280),(x,y+280)],'#b7a386')
poly([(x,y+280),(x+680,y+280),(x+680,y+405),(x,y+405)],'#4d6063')
poly([(x,y),(x+155,y+75),(x+155,y+280),(x,y+405)],'#8e806d')
rect((x+185,y+57,x+402,y+206),'#284b4e')
text(x+206,y+73,'ALGORITHMS LAB',19,'#d7e6d6',True)
for a,b in [((290,139),(238,171)),((290,139),(344,171)),((238,171),(215,193)),((238,171),(264,193))]:
    line([(x+a[0],y+a[1]),(x+b[0],y+b[1])],'#8cb4ad',2)
for a,b in [(290,139),(238,171),(344,171),(215,193),(264,193)]:ellipse((x+a-5,y+b-5,x+a+5,y+b+5),'#dcab71')
rect((x+472,y+41,x+616,y+282),'#61564b')
rect((x+482,y+50,x+606,y+282),'#b4d4d2')
poly([(x+486,y+200),(x+545,y+158),(x+605,y+200),(x+605,y+282),(x+486,y+282)],'#a18b70')
for a in [500,535,570]:rect((x+a,y+212,x+a+15,y+253),'#274352')
poly([(x+482,y+50),(x+448,y+69),(x+448,y+289),(x+482,y+282)],'#576e70')
line([(x+476,y+52),(x+476,y+279)],'#83e0df',4)
tag(x+485,y+9,'COURTYARD',121)
for dx,dy in [(176,267),(348,272),(72,348),(305,366)]:
    poly([(x+dx,y+dy-20),(x+dx+118,y+dy-20),(x+dx+143,y+dy+8),(x+dx-19,y+dy+8)],'#b69b73')
    line([(x+dx,y+dy+8),(x+dx-3,y+dy+37)],'#243b45',6)
    line([(x+dx+125,y+dy+8),(x+dx+131,y+dy+37)],'#243b45',6)
    rect((x+dx+36,y+dy-45,x+dx+88,y+dy-21),'#243e49')
    line([(x+dx+41,y+dy-40),(x+dx+82,y+dy-40)],'#63c2c5',2)
rect((x+198,y+251,x+232,y+269),'#eb8651')
tag(x+20,y+18,'FREE EXPLORATION',190)
tag(x+431,y+353,'LEAVE WHEN READY',217)

# Pursuit: architectural rhythm, a branching path, monkey and short animation beats.
x,y=730,190
panel(x,y,'02','THEFT → CAMPUS PURSUIT','Reach • snatch • turn • run. Follow the monkey through connected campus paths.')
rect((x,y,x+680,y+405),'#28414e')
poly([(x,y+270),(x+680,y+270),(x+680,y+405),(x,y+405)],'#526566')
for side in [0,1]:
    a=x+(0 if side==0 else 475)
    poly([(a,y+35),(a+205,y+95),(a+205,y+285),(a,y+320)],'#9c896d' if side==0 else '#b19b7b')
    for row in range(3):
        for col in range(4):
            xx=a+18+col*47; yy=y+74+row*60+col*10
            rect((xx,yy,xx+19,yy+31),'#263e49')
            line([(xx-3,yy+33),(xx+24,yy+33)],'#c7b793',3)
for ax in [230,370]:
    rect((x+ax,y+140,x+ax+22,y+286),'#bba27e')
    rect((x+ax+3,y+133,x+ax+19,y+148),'#d1bb94')
line([(x+241,y+153),(x+262,y+115),(x+306,y+98),(x+349,y+115),(x+381,y+153)],'#bba27e',18)
rect((x+291,y+40,x+329,y+100),'#ae997b')
rect((x+304,y+53,x+316,y+78),'#26404b')
poly([(x+200,y+405),(x+315,y+275),(x+338,y+275),(x+300,y+405)],'#80918a')
poly([(x+290,y+337),(x+490,y+288),(x+510,y+294),(x+311,y+355)],'#80918a')
line([(x+240,y+388),(x+289,y+341),(x+429,y+303)],'#65d7d6',4)
for ax,ay in [(211,363),(258,322),(352,312)]:
    line([(x+ax,y+ay),(x+ax+11,y+ay-4)],'#ffae6e',5)
monkey(x+405,y+250,1.05)
for xx,yy in [(85,315),(554,321),(178,291)]:
    rect((x+xx,y+yy,x+xx+26,y+yy+32),'#827c65')
    ellipse((x+xx-9,y+yy-15,x+xx+35,y+yy+6),'#427c68')
tag(x+22,y+18,'FOLLOW THE NOTEBOOK',237)
tag(x+398,y+353,'NO FORCED HEAD TURN',256)

# Schematic aerial reveal: pathways and the exact same graph, no geographic claim.
x,y=30,705
panel(x,y,'03','AERIAL REVEAL → TREE SCENE','Campus paths become edges; junctions become nodes. Fade into the playable tree.')
rect((x,y,x+680,y+405),'#253d46')
for a,b,w,h in [(28,57,120,60),(30,201,119,71),(509,46,132,77),(508,203,136,83),(192,23,96,45),(380,23,92,45)]:
    rect((x+a+6,y+b+8,x+a+w+6,y+b+h+8),'#182c35')
    rect((x+a,y+b,x+a+w,y+b+h),'#96886f')
    rect((x+a+9,y+b+9,x+a+w-9,y+b+h-9),'#746f60')
nodes=[(340,332),(340,246),(231,166),(449,166),(178,94),(276,94),(405,94),(492,94)]
edges=[(0,1),(1,2),(1,3),(2,4),(2,5),(3,6),(3,7)]
for a,b in edges:
    p=[(x+nodes[a][0],y+nodes[a][1]),(x+nodes[b][0],y+nodes[b][1])]
    line(p,'#a69b7e',22);line(p,'#88cfce',3)
for i,(a,b) in enumerate(nodes):
    ellipse((x+a-11,y+b-11,x+a+11,y+b+11),'#182b35')
    ellipse((x+a-6,y+b-6,x+a+6,y+b+6),'#f3a264' if i in [0,1,3,6] else '#98d8d5')
line([(x+340,y+332),(x+340,y+246),(x+449,y+166),(x+405,y+94)],'#f2a26a',5)
tag(x+21,y+18,'SCHEMATIC OVERHEAD VIEW',291)
tag(x+20,y+353,'SAME PATHS → SAME TREE',275)
tag(x+385,y+353,'VR: FADE TO FIXED VIEW',270)

# Original tree platform language, with real edge-following locomotion and a live count.
x,y=730,705
panel(x,y,'04','ORIGINAL TREE → PLAYER-DRIVEN CLIMB','Choose once, climb the highlighted branches, count each crossed level, then finish.')
rect((x,y,x+680,y+405),'#142937')
points=[(338,358),(329,276),(400,195),(364,111),(132,280),(530,278),(208,191),(526,114),(189,98),(369,235)]
for a,b in [(0,1),(1,9),(9,2),(2,3),(0,4),(1,5),(1,6),(2,7),(6,8)]:
    line([(x+points[a][0],y+points[a][1]),(x+points[b][0],y+points[b][1])],'#576c77',13)
    line([(x+points[a][0],y+points[a][1]-3),(x+points[b][0],y+points[b][1]-3)],'#75949b',3)
line([(x+338,y+358),(x+329,y+276),(x+369,y+235),(x+400,y+195),(x+364,y+111)],'#73d1d1',6)
for i,(a,b) in enumerate(points):
    c='#ba925b' if i==3 else '#496675'
    rect((x+a-36,y+b,x+a+36,y+b+8),c)
    ellipse((x+a-36,y+b-10,x+a+36,y+b+10),c)
    line([(x+a-23,y+b-6),(x+a+23,y+b-6)],'#ecc489' if i==3 else '#8fbabd',2)
for a,b in [(334,329),(332,305),(347,259),(367,236),(389,213)]:
    line([(x+a-11,y+b),(x+a+11,y+b)],'#f3a060',6)
poly([(x+230,y+399),(x+252,y+341),(x+283,y+302),(x+298,y+313),(x+271,y+359),(x+266,y+399)],'#667c86')
poly([(x+419,y+399),(x+417,y+321),(x+388,y+267),(x+370,y+275),(x+389,y+333),(x+387,y+399)],'#667c86')
ellipse((x+273,y+291,x+310,y+323),'#eca36e')
ellipse((x+356,y+253,x+395,y+284),'#eca36e')
cone(x+364,y+100,.7)
tag(x+20,y+18,'SELECT  1 / 2 / 4 / 8',225)
tag(x+395,y+18,'EXAMPLE: 4-LEVEL ROUTE',264)
rect((x+21,y+89,x+225,y+174),'#243e4a')
text(x+35,y+101,'CLIMBED  2 / 4',24,'#e9eee6',True)
text(x+35,y+138,'2 LEVELS REMAINING',16,'#8cd7d4')
tag(x+20,y+353,'GRIP → PULL → REST',228)
tag(x+452,y+353,'FINISH → RESULTS',206)

im.save(OUT/'revised-experience-storyboard.png')
print(str(OUT/'revised-experience-storyboard.png'))
