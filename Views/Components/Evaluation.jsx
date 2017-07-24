import React from 'react';
import axios from 'axios';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Evaluation extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            evaluation: null,
            error: null
        }
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () =>
            axios.get("/api/evaluation" + this.props.location.search)
                .then(({ data }) => this.setState({ evaluation: data }))
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
        const { evaluation, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>evaluation</strong> you've selected could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (evaluation === null) {
            return null
        }

        return (
            <div>
                <div className="row">
                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Evaluation Properties</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>ID:</strong> {evaluation.id}</li>
                                    <li className="list-group-item"><strong>Job:</strong> {evaluation.jobID}</li>
                                    <li className="list-group-item"><strong>Type:</strong> {evaluation.type}</li>
                                    <li className="list-group-item"><strong>Priority:</strong> {evaluation.priority}</li>
                                    <li className="list-group-item"><strong>Parent:</strong> {evaluation.previousEval}</li>
                                    <li className="list-group-item"><strong>Triggered By:</strong> {evaluation.triggeredBy}</li>
                                    <li className="list-group-item"><strong>Status Description:</strong> {evaluation.statusDescription}</li>
                                    <li className="list-group-item"><strong>Status:</strong> {evaluation.status}</li>
                                </ul>
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
                                    <table className="table table-striped">
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
                                            {evaluation.allocations.sort((a, b) => a.createDateTime < b.createDateTime).map((allocation) => {
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
