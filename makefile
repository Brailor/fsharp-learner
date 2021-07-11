local:
	@echo "hello there"

install-root-deps:
	yarn

bootstrap-fe:
	cd ./client && \
	rm -rf node_modules && \
	yarn

#build frontend app
build-fe:
	cd ./client && \
	yarn build


bootstrap-be:
	cd ./server && \
	dotnet restore

#build backend app
build-be:
	cd ./server && \
	dotnet build
