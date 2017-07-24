import React from 'react';
import axios from 'axios';
import Pager from 'react-pager';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Servers extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            servers: null,
            error: null,
            current: 0
        }

        this.handlePageChanged = this.handlePageChanged.bind(this);
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () => {
            this.props.location.search ?
                axios.get("/api/servers" + this.props.location.search)
                    .then(({ data }) => this.setState({ servers: data }))
                    .catch((error) => this.setState({ error: error })) :
                axios.get("/api/servers")
                    .then(({ data }) => this.setState({ servers: data }))
                    .catch((error) => this.setState({ error: error }))
        }

        this.serverRequest = makeRequest();

        this.poll = setInterval(() => {
            this.serverRequest = makeRequest();
        }, 3000);
    }

    componentWillUnmount() {
        this.serverRequest.abort();
        clearInterval(this.poll);
    }

    handlePageChanged(newPage) {
        this.setState({ current: newPage });
    }

    render() {
        const { servers, error, current } = this.state;
        const itemsPerPage = 15;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the list of <strong>servers</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (servers === null) {
            return null
        }

        var visibleItems = servers.slice(itemsPerPage * current, (itemsPerPage * current) + itemsPerPage);

        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Servers</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-2">Name</th>
                                                <th className="col-sm-2">Address</th>
                                                <th className="col-sm-2">Port</th>
                                                <th className="col-sm-2">Voter</th>
                                                <th className="col-sm-2">Leader</th>
                                                <th className="col-sm-2">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {visibleItems.map((server) => {
                                                return (
                                                    <tr>
                                                        {server.status === "alive" ?
                                                            <td><a href={"/server?ip=" + server.addr}>{server.name}</a></td> :
                                                            <td>{server.name}</td>
                                                        }
                                                        <td>{server.addr}</td>
                                                        <td>{server.port}</td>
                                                        <td>{server.voter.toString()}</td>
                                                        <td>{server.leader.toString()}</td>
                                                        <td>{server.status}</td>
                                                    </tr>
                                                )
                                            })}
                                        </tbody>
                                    </table>
                                </div>

                                {servers.length > 15 ?
                                    <div className="text-center">
                                        <Pager
                                            total={Math.ceil(servers.length / itemsPerPage)}
                                            current={this.state.current}
                                            visiblePages={10}
                                            onPageChanged={this.handlePageChanged}
                                        />
                                    </div> :
                                    <div></div>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
