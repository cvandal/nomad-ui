import React from 'react';
import axios from 'axios';
import Pager from 'react-pager';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Allocations extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            allocations: null,
            error: null,
            current: 0
        }

        this.handlePageChanged = this.handlePageChanged.bind(this);
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () => {
            this.props.location.search ?
                axios.get("/api/allocations" + this.props.location.search)
                    .then(({ data }) => this.setState({ allocations: data }))
                    .catch((error) => this.setState({ error: error })) :
                axios.get("/api/allocations")
                    .then(({ data }) => this.setState({ allocations: data }))
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
        const { allocations, error, current } = this.state;
        const itemsPerPage = 15;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the list of <strong>allocations</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (allocations === null) {
            return null
        }

        var visibleItems = allocations.slice(itemsPerPage * current, (itemsPerPage * current) + itemsPerPage);

        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
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
                                                <th className="col-sm-2">Task Group</th>
                                                <th className="col-sm-2">Desired Status</th>
                                                <th className="col-sm-2">Client Status</th>
                                                <th className="col-sm-2">Create Time</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {visibleItems.map((allocation) => {
                                                return (
                                                    <tr>
                                                        {allocation.desiredStatus === "run" ?
                                                            <td><a href={"/allocation?id=" + allocation.id}>{allocation.id}</a></td> :
                                                            <td>{allocation.id}</td>
                                                        }
                                                        <td>{allocation.name}</td>
                                                        <td>{allocation.taskGroup}</td>
                                                        <td>{allocation.desiredStatus}</td>
                                                        <td>{allocation.clientStatus}</td>
                                                        <td>{moment(allocation.createDateTime).format('DD/MM/YYYY hh:mm:ss A')}</td>
                                                    </tr>
                                                )
                                            })}
                                        </tbody>
                                    </table>
                                </div>

                                {allocations.length > 15 ?
                                    <div className="text-center">
                                        <Pager
                                            total={Math.ceil(allocations.length / itemsPerPage)}
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
