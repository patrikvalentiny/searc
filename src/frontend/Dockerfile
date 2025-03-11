FROM node:lts-slim AS base
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable
COPY src/frontend/ /app
WORKDIR /app

RUN pnpm install --frozen-lockfile --force
RUN pnpm run build

EXPOSE 8000
CMD [ "pnpm", "start" ]