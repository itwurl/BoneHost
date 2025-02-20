# BoneHost

BoneHost is a web application for 3d shape analysis of human bones.

## Quickstart

Just clone repo, build bonehost image and run bonehost container.

```
git clone https://github.com/itwurl/boneHost-development
cd bonehost
docker build -t bonehost .
docker run -d -p 80:80 bonehost
```

To test bonehost open localhost:80 with a web browser.

