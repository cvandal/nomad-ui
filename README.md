# Nomad UI

Something something something Hashicorp doesn't provide a free UI for Nomad something.

### Prerequisites
Enable CORS on the HTTP API endpoint by adding the following to your Nomad agent configuration:
```
http_api_response_headers {
    "Access-Control-Allow-Origin" = "*"
}
```

### Usage
```
docker run -itd -e "NOMAD_URL=http://<base_url>:<port>" -e "ASPNETCORE_URLS=http://*:5000" -p 5000:5000 cvandal/nomad-ui:0.2.0
```

### Development
1. Run `git clone git@github.com:cvandal/nomad-ui.git` or `git clone https://github.com/cvandal/nomad-ui.git`
2. Run `dotnet restore`, followed by `dotnet build`, followed by `dotnet publish`
3. Run `cd .\Nomad\bin\Debug\netcoreapp1.1\publish\` and create a `Dockerfile` in the publish output directory with the following content:
```
FROM microsoft/dotnet

COPY . /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Nomad.dll"]
```
4. Run `docker build <image_name>:<image_tag> .`
5. Run `docker run -itd -e "NOMAD_URL=http://<base_url>:<port>" -e "ASPNETCORE_URLS=http://*:5000" -p 5000:5000 <image_name>:<image_tag>`

### Screenshots
![alt text](https://github.com/cvandal/nomad-ui/raw/master/Nomad/wwwroot/images/dashboard.png "Dashboard")
![alt text](https://github.com/cvandal/nomad-ui/raw/master/Nomad/wwwroot/images/job.png "Job")
![alt text](https://github.com/cvandal/nomad-ui/raw/master/Nomad/wwwroot/images/allocation.png "Allocation")
![alt text](https://github.com/cvandal/nomad-ui/raw/master/Nomad/wwwroot/images/client.png "Client")

### Known Issues
`¯\_(ツ)_/¯`
