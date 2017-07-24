import React from 'react';
import axios from 'axios';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Server extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            server: null,
            error: null
        }
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () =>
            axios.get("/api/server" + this.props.location.search)
                .then(({ data }) => this.setState({ server: data }))
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
        const { server, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>server</strong> you've selected could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (server === null) {
            return null
        }

        return (
            <div>
                <div className="row">
                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Server Properties</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>Name:</strong> {server.member.name}</li>
                                    <li className="list-group-item"><strong>Address:</strong> {server.member.addr}</li>
                                    <li className="list-group-item"><strong>Port:</strong> {server.member.port}</li>
                                    <li className="list-group-item">
                                        {server.member.operator.servers.map((s) => {
                                            return (
                                                <div>
                                                    {server.member.name === s.node ? <span><strong>Voter:</strong> {s.voter.toString()}</span> : <span></span>}
                                                </div>
                                            )
                                        })}
                                    </li>
                                    <li className="list-group-item">
                                        {server.member.operator.servers.map((s) => {
                                            return (
                                                <div>
                                                    {server.member.name === s.node ? <span><strong>Leader:</strong> {s.leader.toString()}</span> : <span></span>}
                                                </div>
                                            )
                                        })}
                                    </li>
                                    <li className="list-group-item"><strong>Status:</strong> {server.member.status}</li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Server Configuration</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>Region:</strong> {server.config.region}</li>
                                    <li className="list-group-item"><strong>Datacenter:</strong> {server.config.datacenter}</li>
                                    <li className="list-group-item"><strong>Data Directory:</strong> {server.config.dataDir}</li>
                                    <li className="list-group-item">
                                        <strong>Files:</strong>
                                        {server.config.files.map((file) => {
                                            return (
                                                <ul className="list-group list-group-condensed">
                                                    <li className="list-group-item list-group-item-condensed">{file}</li>
                                                </ul>
                                            )
                                        })}
                                    </li>
                                    <li className="list-group-item"><strong>Log Level:</strong> {server.config.logLevel}</li>
                                    <li className="list-group-item"><strong>Bind Address:</strong> {server.config.bindAddr}</li>
                                    <li className="list-group-item">
                                        <strong>Advertise Addresses:</strong>
                                        <ul className="list-group list-group-condensed">
                                            <li className="list-group-item list-group-item-condensed">{server.config.advertiseAddrs.http}</li>
                                            <li className="list-group-item list-group-item-condensed">{server.config.advertiseAddrs.rpc}</li>
                                            <li className="list-group-item list-group-item-condensed">{server.config.advertiseAddrs.serf}</li>
                                        </ul>
                                    </li>
                                    <li className="list-group-item"><strong>Nomad Version:</strong> {server.config.version}</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
