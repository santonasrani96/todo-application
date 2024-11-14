FROM node:18-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build

ENV BASE_API_URL=http://localhost:5108/api/

EXPOSE 3000

CMD ["npm", "run", "dev"]
