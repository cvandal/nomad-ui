import React from 'react';
import axios from 'axios';
import Pager from 'react-pager';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Clients extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            clients: null,
            error: null,
            current: 0
        }

        this.handlePageChanged = this.handlePageChanged.bind(this);
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () => {
            this.props.location.search ?
                axios.get("/api/clients" + this.props.location.search)
                    .then(({ data }) => this.setState({ clients: data }))
                    .catch((error) => this.setState({ error: error })) :
                axios.get("/api/clients")
                    .then(({ data }) => this.setState({ clients: data }))
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
        const { clients, error, current } = this.state;
        const itemsPerPage = 15;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the list of <strong>clients</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (clients === null) {
            return null
        }

        var visibleItems = clients.slice(itemsPerPage * current, (itemsPerPage * current) + itemsPerPage);

        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Clients</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-3">ID</th>
                                                <th className="col-sm-3">Name</th>
                                                <th className="col-sm-2">Datacenter</th>
                                                <th className="col-sm-2">Drain</th>
                                                <th className="col-sm-2">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {visibleItems.map((client) => {
                                                return (
                                                    <tr>
                                                        {client.status === "ready" ?
                                                            <td><a href={"/client?id=" + client.id}>{client.id}</a></td> :
                                                            <td>{client.id}</td>
                                                        }
                                                        <td>{client.name}</td>
                                                        <td>{client.datacenter}</td>
                                                        <td>{client.drain.toString()}</td>
                                                        <td>{client.status}</td>
                                                    </tr>
                                                )
                                            })}
                                        </tbody>
                                    </table>
                                </div>

                                {clients.length > 15 ?
                                    <div className="text-center">
                                        <Pager
                                            total={Math.ceil(clients.length / itemsPerPage)}
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
