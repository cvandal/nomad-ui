import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import moment from 'moment';
import ReactPaginate from 'react-paginate';

interface FetchData {
    evaluations: any[];
    currentPage: number;
    itemsPerPage: number;
    loading: boolean;
}

export class Evaluations extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { evaluations: [], currentPage: 0, itemsPerPage: 15, loading: true};
    }

    fetchData = (search) => {
        fetch('api/evaluations' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ evaluations: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Evaluations";

        this.fetchData(this.props.location.search);
        this.poll = setInterval(() => this.fetchData(this.props.location.search), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.poll);
    }

    handlePageChange = (data) => {
        let nextPage = Math.ceil(data.selected * this.state.itemsPerPage);
        this.setState({ currentPage: nextPage });
    }

    public render() {
        let visibleItems = this.state.evaluations.slice(this.state.currentPage, this.state.currentPage + this.state.itemsPerPage);

        let evaluations = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Evaluations.renderEvaluations(visibleItems);

        return <div className="container-fluid">
            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Evaluations</h3>
                        </div>
                        <div className="panel-body">
                            { evaluations }

                            { this.state.evaluations.length > this.state.itemsPerPage &&
                                <div className="text-center">
                                    <ReactPaginate 
                                        pageCount={Math.ceil(this.state.evaluations.length / this.state.itemsPerPage)}
                                        pageRangeDisplayed={9}
                                        marginPagesDisplayed={3}
                                        onPageChange={this.handlePageChange}
                                        previousLabel={<span>&laquo;</span>}
                                        nextLabel={<span>&raquo;</span>}
                                        containerClassName={'pagination'}
                                        activeClassName={'active'}
                                        breakLabel={<span>...</span>}
                                    />
                                </div>
                            }
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }

    private static renderEvaluations(evaluations) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-2">Evaluation ID</th>
                        <th className="col-sm-2">Job ID</th>
                        <th className="col-sm-2">Type</th>
                        <th className="col-sm-2">Priority</th>
                        <th className="col-sm-2">Triggered By</th>
                        <th className="col-sm-2">Status</th>
                    </tr>
                </thead>
                <tbody>
                    {evaluations.map(evaluation =>
                        <tr>
                            <td><a href={"/evaluation?id=" + evaluation.ID}>{ evaluation.ID }</a></td>
                            <td><a href={"/job?id=" + evaluation.JobID}>{ evaluation.JobID }</a></td>
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
}
