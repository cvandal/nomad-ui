import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import moment from 'moment';

interface FetchData {
    evaluation: any;
    evaluationAllocations: any[];
    loading: boolean;
}

export class Evaluation extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { evaluation: null, evaluationAllocations: [], loading: true};
    }

    fetchData = (search) => {
        fetch('api/evaluation' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ evaluation: data, loading: false });
            });
        fetch('api/evaluation/allocations' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ evaluationAllocations: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Evaluation";

        this.fetchData(this.props.location.search);
        this.poll = setInterval(() => this.fetchData(this.props.location.search), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.poll);
    }

    public render() {
        let evaluation = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Evaluation.renderEvaluation(this.state.evaluation);

        let evaluationAllocations = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Evaluation.renderEvaluationAllocations(this.state.evaluationAllocations);

        return <div className="container-fluid">
            { evaluation }

            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Allocations</h3>
                        </div>
                        <div className="panel-body">
                            { evaluationAllocations }
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }

    private static renderEvaluation(evaluation) {
        return <div>
            <div className="row">
                <div className="col-sm-4">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Evaluation Properties</h3>
                        </div>
                        <div className="panel-body">
                            <ul className="list-group">
                                <li className="list-group-item"><strong>ID:</strong> { evaluation.ID }</li>
                                <li className="list-group-item"><strong>Previous Evaluation ID:</strong> { evaluation.PreviousEval ? evaluation.PreviousEval : "N/A" }</li>
                                <li className="list-group-item"><strong>Job ID:</strong> <a href={"/job?id=" + evaluation.JobID}>{ evaluation.JobID }</a></li>
                                <li className="list-group-item"><strong>Type:</strong> { evaluation.Type }</li>
                                <li className="list-group-item"><strong>Priority:</strong> { evaluation.Priority }</li>
                                <li className="list-group-item"><strong>Triggered By:</strong> { evaluation.TriggeredBy }</li>
                                <li className="list-group-item"><strong>Status Description:</strong> { evaluation.StatusDescription ? evaluation.StatusDescription : "N/A" }</li>
                                <li className="list-group-item"><strong>Status:</strong> { evaluation.Status }</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }

    private static renderEvaluationAllocations(evaluationAllocations) {
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
                    {evaluationAllocations.sort((a, b) => b.CreateTime > a.CreateTime).map(allocation =>
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
