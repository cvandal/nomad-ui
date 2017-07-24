import React from 'react';
import axios from 'axios';
import {
    defaults,
    Doughnut
} from 'react-chartjs-2';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Client extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            client: null,
            error: null
        }
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () =>
            axios.get("/api/client" + this.props.location.search)
                .then(({ data }) => this.setState({ client: data }))
                .catch((error) => this.setState({ error: error }))

        this.serverRequest = makeRequest();

        this.poll = setInterval(() => {
            this.serverRequest = makeRequest();
        }, 3000);
    }

    componentWillUnmount() {
        this.serverRequest.abort();
        clearInterval(this.poll);
    }

    render() {
        const { client, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>client</strong> you've selected could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (client === null) {
            return null
        }

        return (
            <div>
                <div className="row">
                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Client Properties</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>ID:</strong> {client.id}</li>
                                    <li className="list-group-item"><strong>Name:</strong> {client.name}</li>
                                    <li className="list-group-item"><strong>Datacenter:</strong> {client.datacenter}</li>
                                    <li className="list-group-item"><strong>Address:</strong> {client.httpAddr}</li>
                                    <li className="list-group-item"><strong>TLS:</strong> {client.tlsEnabled.toString()}</li>
                                    <li className="list-group-item"><strong>Drain:</strong> {client.drain.toString()}</li>
                                    <li className="list-group-item"><strong>Status Description:</strong> {client.statusDescription}</li>
                                    <li className="list-group-item"><strong>Status:</strong> {client.status}</li>
                                    <li className="list-group-item">
                                        <strong>Meta:</strong>
                                        {client.meta ?
                                            <ul className="list-group list-group-condensed">
                                                {Object.entries(client.meta).map(([key, value]) => {
                                                    return (
                                                        <li className="list-group-item list-group-item-condensed">{key} = {value}</li>
                                                    )
                                                })}
                                            </ul> :
                                            <p></p>
                                        }
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Client Resources</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>CPU (MHz):</strong> {client.resources.cpu}</li>
                                    <li className="list-group-item"><strong>Memory (MB):</strong> {client.resources.memoryMB}</li>
                                    <li className="list-group-item"><strong>Disk (MB):</strong> {client.resources.diskMB}</li>
                                    <li className="list-group-item">
                                        {client.resources.networks.map((network) => {
                                            return (
                                                <div><strong>Network (Mpbs):</strong> {network.mBits}</div>
                                            )
                                        })}
                                    </li>
                                    <li className="list-group-item">
                                        {client.resources.networks.map((network) => {
                                            return (
                                                <div><strong>IP Address:</strong> {network.ip}</div>
                                            )
                                        })}
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Client Attributes</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>Operating System:</strong> {client.attributes['os.name']} {client.attributes['os.version']}</li>
                                    <li className="list-group-item"><strong>Docker Version:</strong> {client.attributes['driver.docker.version']}</li>
                                    <li className="list-group-item"><strong>Nomad Version:</strong> {client.attributes['nomad.version']}</li>
                                    <li className="list-group-item"><strong>Consul Version:</strong> {client.attributes['consul.version']}</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    {client.stats.cpu.map((cpu) => {
                        return (
                            <div className="col-sm-3">
                                <div className="panel panel-default">
                                    <div className="panel-heading">
                                        <h3 className="panel-title text-center">{cpu.cpu === "" ? <span>CPU0</span> : <span>{cpu.cpu.toUpperCase()}</span>} Utilisation</h3>
                                    </div>
                                    <div className="panel-body">
                                        <Doughnut
                                            data={{
                                                labels: ['Idle %', 'Active %'],
                                                datasets: [{
                                                    data: [cpu.idle, cpu.total],
                                                    backgroundColor: ['#1fb58f', '#eab126'],
                                                    hoverBackgroundColor: ['#1fb58f', '#eab126'],
                                                    borderColor: ['#1e1e1e', '#1e1e1e']
                                                }]
                                            }}
                                            options={{
                                                cutoutPercentage: 75,
                                                legend: {
                                                    position: 'left',
                                                    labels: {
                                                        fontColor: "#fff"
                                                    }
                                                },
                                                animation: {
                                                    animateScale: false,
                                                    animateRotate: false
                                                }
                                            }}
                                            height={200}
                                        />
                                    </div>
                                </div>
                            </div>
                        )
                    })}

                    <div className="col-sm-3">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Memory Utilisation</h3>
                            </div>
                            <div className="panel-body">
                                <Doughnut
                                    data={{
                                        labels: ['Available MB', 'Consumed MB'],
                                        datasets: [{
                                            data: [client.stats.memory.available, client.stats.memory.used],
                                            backgroundColor: ['#1fb58f', '#eab126'],
                                            hoverBackgroundColor: ['#1fb58f', '#eab126'],
                                            borderColor: ['#1e1e1e', '#1e1e1e']
                                        }]
                                    }}
                                    options={{
                                        cutoutPercentage: 75,
                                        legend: {
                                            position: 'left',
                                            labels: {
                                                fontColor: "#fff"
                                            }
                                        },
                                        animation: {
                                            animateScale: false,
                                            animateRotate: false
                                        }
                                    }}
                                    height={200}
                                />
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Allocations</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-2">ID</th>
                                                <th className="col-sm-2">Name</th>
                                                <th className="col-sm-1">Task Group</th>
                                                <th className="col-sm-1">Desired Status</th>
                                                <th className="col-sm-2">Client</th>
                                                <th className="col-sm-1">Client Status</th>
                                                <th className="col-sm-1">Create Time</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {client.allocations.sort((a, b) => a.createDateTime < b.createDateTime).map((allocation) => {
                                                return (
                                                    <tr>
                                                        <td><a href={"/allocation?id=" + allocation.id}>{allocation.id}</a></td>
                                                        <td>{allocation.name}</td>
                                                        <td>{allocation.taskGroup}</td>
                                                        <td>{allocation.desiredStatus}</td>
                                                        <td><a href={"/client?id=" + allocation.nodeID}>{allocation.nodeID}</a></td>
                                                        <td>{allocation.clientStatus}</td>
                                                        <td>{moment(allocation.createDateTime).format('DD/MM/YYYY hh:mm:ss A')}</td>
                                                    </tr>
                                                )
                                            })}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
