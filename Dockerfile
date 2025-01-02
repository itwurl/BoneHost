# use official nginx image
FROM nginx:alpine

# copy data into nginx document root directory
COPY WebGL /usr/share/nginx/html/

# set working directory
WORKDIR /usr/share/nginx/html

# set port container will listen on
EXPOSE 80

# start nginx webserver in foreground
CMD ["nginx", "-g", "daemon off;"]
