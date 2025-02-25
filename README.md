# BoneHost

BoneHost is a web application for 3d shape analysis of human bones.

## Quickstart

Clone the repo, build the Bonehost image, and run the container.

```
git clone https://github.com/itwurl/BoneHost.git
cd bonehost
docker build -t bonehost .
docker run -d -p 80:80 bonehost
```

To test bonehost open localhost:80 with a web browser.

