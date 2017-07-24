# Nomad UI

A UI for Nomad by HashiCorp.

## Usage
1. Run `docker run -itd -e "NOMAD_URL=http://<base_url>:<port>" -e "ASPNETCORE_URLS=http://*:5000" -p 5000:5000 cvandal/nomad-ui:1.0.0`

## Development
### Prerequisites
1. Download and install the latest version of .NET Core from https://www.microsoft.com/net/download/core
2. Download and install the latest version of Node.js from https://nodejs.org/en/download/
3. Download and install the latest version of Yarn from https://yarnpkg.com/en/docs/install
4. Clone the repository from GitHub by running `git clone https://github.com/cvandal/nomad-ui.git` 

### Build
1. Run `yarn`, followed by `.\node_modules\.bin\webpack`
2. Run `dotnet restore`, followed by `dotnet build`, followed by `dotnet publish`
3. Run `cd .\bin\Debug\netcoreapp1.1\publish\` and create a file named `Dockerfile` with the following content:
```
FROM microsoft/dotnet

COPY . /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Nomad.dll"]
```
4. Run `docker build -t <image_name>:<image_tag> .`
### Run
1. Run `docker run -itd -e "NOMAD_URL=http://<base_url>:<port>" -e "ASPNETCORE_URLS=http://*:5000" -p 5000:5000 <image_name>:<image_tag>`

## Screenshots
![alt text](https://github.com/cvandal/nomad-ui/raw/master/wwwroot/images/collage.png "Nomad UI")

## Known Issues
`¯\_(ツ)_/¯`
