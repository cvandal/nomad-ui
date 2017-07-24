import React from 'react';
import axios from 'axios';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Job extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            job: null,
            error: null
        }
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () =>
            axios.get("/api/job" + this.props.location.search)
                .then(({ data }) => this.setState({ job: data }))
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
        const { job, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>job</strong> you've selected could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (job === null) {
            return null
        }

        return (
            <div>
                <div className="row">
                    <div className="col-sm-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Job Properties</h3>
                            </div>
                            <div className="panel-body">
                                <ul className="list-group">
                                    <li className="list-group-item"><strong>ID:</strong> {job.id}</li>
                                    <li className="list-group-item"><strong>Name:</strong> {job.name}</li>
                                    <li className="list-group-item"><strong>Region:</strong> {job.region}</li>
                                    <li className="list-group-item">
                                        <strong>Datacenters:</strong>
                                        {job.datacenters.map((datacenter) => {
                                            return (
                                                <ul className="list-group list-group-condensed">
                                                    <li className="list-group-item list-group-item-condensed">{datacenter}</li>
                                                </ul>
                                            )
                                        })}
                                    </li>
                                    <li className="list-group-item"><strong>Type:</strong> {job.type}</li>
                                    <li className="list-group-item"><strong>Priority:</strong> {job.priority}</li>
                                    <li className="list-group-item"><strong>Status:</strong> {job.status}</li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Job Constraints</h3>
                            </div>
                            <div className="panel-body">
                                {job.constraints ?
                                    <ul className="list-group">
                                        {job.constraints.map((constraint) => {
                                            return (
                                                <li className="list-group-item">{constraint.lTarget} {constraint.operand} {constraint.rTarget}</li>
                                            )
                                        })}
                                    </ul> :
                                    <p></p>
                                }
                            </div>
                        </div>
                    </div>

                    <div className="col-md-4">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Job Meta</h3>
                            </div>
                            <div className="panel-body">
                                {job.meta ?
                                    <ul className="list-group">
                                        {Object.entries(job.meta).sort().map(([key, value]) => {
                                            return (
                                                <li className="list-group-item">{key} = {value}</li>
                                            )
                                        })}
                                    </ul> :
                                    <p></p>
                                }
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Task Groups</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-striped">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-1">Name</th>
                                                <th className="col-sm-1">Count</th>
                                                <th className="col-sm-1">Tasks</th>
                                                <th className="col-sm-1">Restart Policy</th>
                                                <th className="col-sm-1">Restart Attempts</th>
                                                <th className="col-sm-1">Restart Delay</th>
                                                <th className="col-sm-2">Constraints</th>
                                                <th className="col-sm-2">Meta</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {job.taskGroups.map((taskGroup) => {
                                                return (
                                                    <tr>
                                                        <td>{taskGroup.name}</td>
                                                        <td>{taskGroup.count}</td>
                                                        <td>{taskGroup.tasks.length}</td>
                                                        <td>{taskGroup.restartPolicy.mode}</td>
                                                        <td>{taskGroup.restartPolicy.attempts}</td>
                                                        <td>{taskGroup.restartPolicy.delay}</td>
                                                        <td>
                                                            {taskGroup.constraints ?
                                                                <ul className="list-group-condensed">
                                                                    {taskGroup.constraints.map((constraint) => {
                                                                        return (
                                                                            <li>{constraint.lTarget} {constraint.operand} {constraint.rTarget}</li>
                                                                        )
                                                                    })}
                                                                </ul> :
                                                                <p></p>
                                                            }
                                                        </td>
                                                        <td>
                                                            {taskGroup.meta ?
                                                                <ul className="list-group-condensed">
                                                                    {Object.entries(taskGroup.meta).sort().map(([key, value]) => {
                                                                        return (
                                                                            <li>{key} = {value}</li>
                                                                        )
                                                                    })}
                                                                </ul> :
                                                                <p></p>
                                                            }
                                                        </td>
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

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Tasks</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-striped">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-1">Name</th>
                                                <th className="col-sm-1">Task Group</th>
                                                <th className="col-sm-1">Driver</th>
                                                <th className="col-sm-1">CPU (MHz)</th>
                                                <th className="col-sm-1">Memory (MB)</th>
                                                <th className="col-sm-1">Network (Mbps)</th>
                                                <th className="col-sm-2">Constraints</th>
                                                <th className="col-sm-2">Meta</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {job.taskGroups.map((taskGroup) => taskGroup.tasks.map((task) => (
                                                <tr>
                                                    <td>{task.name}</td>
                                                    <td>{taskGroup.name}</td>
                                                    <td>{task.driver}</td>
                                                    <td>{task.resources.cpu}</td>
                                                    <td>{task.resources.memoryMB}</td>
                                                    <td>
                                                        {task.resources.networks.map((network) => {
                                                            return (
                                                                <span>{network.mBits}</span>
                                                            )
                                                        })}
                                                    </td>
                                                    <td>
                                                        {task.constraints ?
                                                            <ul className="list-group-condensed">
                                                                {task.constraints.map((constraint) => {
                                                                    return (
                                                                        <li>{constraint.lTarget} {constraint.operand} {constraint.rTarget}</li>
                                                                    )
                                                                })}
                                                            </ul> :
                                                            <p></p>
                                                        }
                                                    </td>
                                                    <td>
                                                        {task.meta ?
                                                            <ul className="list-group-condensed">
                                                                {Object.entries(task.meta).sort().map(([key, value]) => {
                                                                    return (
                                                                        <li>{key} = {value}</li>
                                                                    )
                                                                })}
                                                            </ul> :
                                                            <p></p>
                                                        }
                                                    </td>
                                                </tr>
                                            ))).reduce((flattened, rows) => [...flattened, ...rows], [])}
                                       </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Docker Properties</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-striped">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-1">Task</th>
                                                <th className="col-sm-3">Image</th>
                                                <th className="col-sm-1">Network</th>
                                                <th className="col-sm-1">Command</th>
                                                <th className="col-sm-4">Arguments</th>
                                                <th className="col-sm-3">Environment Variables</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {job.taskGroups.map((taskGroup) => taskGroup.tasks.map((task) => (
                                                <tr>
                                                    <td>{task.name}</td>
                                                    <td>{task.config.image}</td>
                                                    <td>{task.config.network_Mode}</td>
                                                    <td>{task.config.command}</td>
                                                    <td>
                                                        {task.config.args ?
                                                            <ul className="list-group-condensed">
                                                                {task.config.args.map((arg) => {
                                                                    return (
                                                                        <li>{arg}</li>
                                                                    )
                                                                })}
                                                            </ul> :
                                                            <p></p>
                                                        }
                                                    </td>
                                                    <td>
                                                        {task.env ?
                                                            <ul className="list-group-condensed">
                                                                {Object.entries(task.env).sort().map(([key, value]) => {
                                                                    return (
                                                                        <li>{key} = {value}</li>
                                                                    )
                                                                })}
                                                            </ul> :
                                                            <p></p>
                                                        }
                                                    </td>
                                                </tr>
                                            ))).reduce((flattened, rows) => [...flattened, ...rows], [])}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Evaluations</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-striped">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-2">ID</th>
                                                <th className="col-sm-2">Parent ID</th>
                                                <th className="col-sm-2">Job</th>
                                                <th className="col-sm-1">Type</th>
                                                <th className="col-sm-1">Priority</th>
                                                <th className="col-sm-1">Triggered By</th>
                                                <th className="col-sm-1">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {job.evaluations.map((evaluation) => {
                                                return (
                                                    <tr>
                                                        <td><a href={"/evaluation?id=" + evaluation.id}>{evaluation.id}</a></td>
                                                        <td>{evaluation.previousEval}</td>
                                                        <td>{evaluation.jobID}</td>
                                                        <td>{evaluation.type}</td>
                                                        <td>{evaluation.priority}</td>
                                                        <td>{evaluation.triggeredBy}</td>
                                                        <td>{evaluation.status}</td>
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
                                            {job.allocations.sort((a, b) => a.createDateTime < b.createDateTime).map((allocation) => {
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
