# ---- build ----
FROM node:22 AS build
WORKDIR /app

# O npm 10 que vem no node:22 falha ao resolver as peer deps do vitest/jsdom
# ("Cannot read properties of null (reading 'edgesOut')"); o lockfile foi gerado com npm 11.
RUN npm install -g npm@11

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---- runtime ----
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/prescricao-web/browser /usr/share/nginx/html
EXPOSE 80
