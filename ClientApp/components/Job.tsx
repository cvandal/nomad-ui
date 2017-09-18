import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import moment from 'moment';

interface FetchData {
    job: any;
    jobEvaluations: any[];
    jobAllocations: any[];
    loading: boolean;
}

export class Job extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { job: null, jobEvaluations: [], jobAllocations: [], loading: true};
    }

    fetchData = (search) => {
        fetch('api/job' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ job: data, loading: false });
            });
        fetch('api/job/evaluations' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ jobEvaluations: data, loading: false });
            });
        fetch('api/job/allocations' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ jobAllocations: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Job";

        this.fetchData(this.props.location.search);
        this.poll = setInterval(() => this.fetchData(this.props.location.search), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.poll);
    }

    public render() {
        let job = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Job.renderJob(this.state.job);

        let jobEvaluations = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Job.renderJobEvaluations(this.state.jobEvaluations);

        let jobAllocations = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Job.renderJobAllocations(this.state.jobAllocations);

        return <div className="container-fluid">
            { job }

            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Evaluations</h3>
                        </div>
                        <div className="panel-body">
                            { jobEvaluations }
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
                            { jobAllocations }
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }

    private static renderJob(job: any) {
        return <div>
            <div className="row">
                <div className="col-sm-4">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Job Properties</h3>
                        </div>
                        <div className="panel-body">
                            <ul className="list-group">
                                <li className="list-group-item"><strong>ID:</strong> { job.ID }</li>
                                <li className="list-group-item"><strong>Name:</strong> { job.Name }</li>
                                <li className="list-group-item"><strong>Region:</strong> { job.Region }</li>
                                <li className="list-group-item">
                                    <strong>Datacenters:</strong>
                                    <ul className="list-group list-group-child">
                                        {job.Datacenters.map(datacenter =>
                                            <li className="list-group-item list-group-item-child">{ datacenter }</li>
                                        )}
                                    </ul>
                                </li>
                                <li className="list-group-item"><strong>Type:</strong> { job.Type }</li>
                                <li className="list-group-item"><strong>Priority:</strong> { job.Priority }</li>
                                <li className="list-group-item"><strong>Status:</strong> { job.Status }</li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div className="col-sm-4">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Job Constraints</h3>
                        </div>
                        <div className="panel-body">
                            {job.Constraints
                                ? <ul className="list-group">
                                    {job.Constraints.sort((a, b) => a.LTarget > b.LTarget).map(constraint =>
                                        <li className="list-group-item">{ constraint.LTarget } { constraint.Operand } { constraint.RTarget }</li>
                                    )}
                                </ul>
                                : <p className="text-center">N/A</p>
                            }
                        </div>
                    </div>
                </div>

                <div className="col-sm-4">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Job Meta</h3>
                        </div>
                        <div className="panel-body">
                            {job.Meta
                                ? <ul className="list-group">
                                    {Object.entries(job.Meta).sort().map(([key, value]) => {
                                        return (
                                            <li className="list-group-item">{ key } = { value }</li>
                                        )
                                    })}
                                </ul>
                                : <p className="text-center">N/A</p>
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
                                            <th className="col-sm-1">Restart Mode</th>
                                            <th className="col-sm-1">Restart Attempts</th>
                                            <th className="col-sm-1">Restart Delay</th>
                                            <th className="col-sm-2">Constraints</th>
                                            <th className="col-sm-2">Meta</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {job.TaskGroups.sort((a, b) => a.Name > b.Name).map(taskGroup =>
                                            <tr>
                                                <td>{ taskGroup.Name }</td>
                                                <td>{ taskGroup.Count }</td>
                                                <td>{ taskGroup.Tasks.length }</td>
                                                <td>{ taskGroup.RestartPolicy.Mode }</td>
                                                <td>{ taskGroup.RestartPolicy.Attempts }</td>
                                                <td>{ taskGroup.RestartPolicy.Delay }</td>
                                                <td>
                                                    {taskGroup.Constraints
                                                        ? <ul className="list-group">
                                                            {taskGroup.Constraints.map(constraint =>
                                                                <li className="list-group-item">{constraint.LTarget} {constraint.Operand} {constraint.RTarget}</li>
                                                            )}
                                                        </ul>
                                                        : "N/A"
                                                    }
                                                </td>
                                                <td>
                                                    {taskGroup.Meta
                                                        ? <ul className="list-group">
                                                            {Object.entries(taskGroup.Meta).sort().map(([key, value]) => {
                                                                return (
                                                                    <li className="list-group-item">{key} = {value}</li>
                                                                )
                                                            })}
                                                        </ul>
                                                        : "N/A"
                                                    }
                                                </td>
                                            </tr>
                                        )}
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
                                            <th className="col-sm-1">Memory(MB)</th>
                                            <th className="col-sm-1">Network (Mbps)</th>
                                            <th className="col-sm-2">Constraints</th>
                                            <th className="col-sm-2">Meta</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {job.TaskGroups.map(taskGroup => taskGroup.Tasks.sort((a, b) => a.Name > b.Name).map(task => (
                                            <tr>
                                                <td>{ task.Name }</td>
                                                <td>{ taskGroup.Name }</td>
                                                <td>{ task.Driver }</td>
                                                <td>{ task.Resources.CPU }</td>
                                                <td>{ task.Resources.MemoryMB }</td>
                                                <td>{ task.Resources.Networks[0].MBits}</td>
                                                <td>
                                                    {task.Constraints
                                                        ? <ul className="list-group">
                                                            {task.Constraints.map(constraint =>
                                                                <li className="list-group-item">{constraint.LTarget} {constraint.Operand} {constraint.RTarget}</li>
                                                            )}
                                                        </ul>
                                                        : "N/A"
                                                    }
                                                </td>
                                                <td>
                                                    {task.Meta
                                                        ? <ul className="list-group">
                                                            {Object.entries(task.Meta).sort().map(([key, value]) => {
                                                                return (
                                                                    <li className="list-group-item">{key} = {value}</li>
                                                                )
                                                            })}
                                                        </ul>
                                                        : "N/A"
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
                            <h3 className="panel-title text-center">Containers</h3>
                        </div>
                        <div className="panel-body">
                            <div className="table-responsive">
                                <table className="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>Task</th>
                                            <th>Image</th>
                                            <th>Command</th>
                                            <th>Arguments</th>
                                            <th>Environment Variables</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {job.TaskGroups.map(taskGroup => taskGroup.Tasks.sort((a, b) => a.Name > b.Name).map(task => (
                                            <tr>
                                                <td>{ task.Name }</td>
                                                <td>{ task.Config.image }</td>
                                                <td>{ task.Config.command ? task.Config.command : "N/A"}</td>
                                                <td>
                                                    {task.Config.args
                                                        ? <ul className="list-group">
                                                            {task.Config.args.map(arg =>
                                                                <li className="list-group-item">{arg}</li>
                                                            )}
                                                        </ul>
                                                        : "N/A"
                                                    }
                                                </td>
                                                <td>
                                                    {task.Env
                                                        ? <ul className="list-group">
                                                            {Object.entries(task.Env).sort().map(([key, value]) => {
                                                                return (
                                                                    <li className="list-group-item">{key} = {value}</li>
                                                                )
                                                            })}
                                                        </ul>
                                                        : "N/A"
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
        </div>;
    }

    private static renderJobEvaluations(jobEvaluations) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-2">ID</th>
                        <th className="col-sm-2">Previous Evaluation ID</th>
                        <th className="col-sm-1">Type</th>
                        <th className="col-sm-2">Priority</th>
                        <th className="col-sm-2">Triggered By</th>
                        <th className="col-sm-1">Status</th>
                    </tr>
                </thead>
                <tbody>
                    {jobEvaluations.sort((a, b) => a.ID > b.ID).map(evaluation =>
                        <tr>
                            <td><a href={"/evaluation?id=" + evaluation.ID}>{ evaluation.ID }</a></td>
                            <td><a href={"/evaluation?id=" + evaluation.PreviousEval}>{ evaluation.PreviousEval }</a></td>
                            <td>{ evaluation.Type }</td>
                            <td>{ evaluation.Priority }</td>
                            <td>{ evaluation.TriggeredBy }</td>
                            <td>{ evaluation.Status }</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }

    private static renderJobAllocations(jobAllocations) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-2">ID</th>
                        <th className="col-sm-2">Name</th>
                        <th className="col-sm-1">Desired Status</th>
                        <th className="col-sm-2">Client</th>
                        <th className="col-sm-2">Client Status</th>
                        <th className="col-sm-1">Create Time</th>
                    </tr>
                </thead>
                <tbody>
                    {jobAllocations.sort((a, b) => b.CreateTime > a.CreateTime).map(allocation =>
                        <tr>
                            <td><a href={"/allocation?id=" + allocation.ID}>{ allocation.ID }</a></td>
                            <td>{ allocation.Name }</td>
                            <td>{ allocation.DesiredStatus }</td>
                            <td><a href={"/client?id=" + allocation.NodeID}>{ allocation.NodeID }</a></td>
                            <td>{ allocation.ClientStatus }</td>
                            <td>{ moment(allocation.CreateTime).format('DD/MM/YYYY hh:mm:ss A') }</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}
