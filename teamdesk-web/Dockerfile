# ---- Étape 1 : compiler Angular ----
FROM node:lts AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npx ng build

# ---- Étape 2 : servir les fichiers avec Nginx ----
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/teamdesk-web/browser /usr/share/nginx/html
EXPOSE 80