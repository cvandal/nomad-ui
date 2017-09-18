import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import moment from 'moment';
import ReactPaginate from 'react-paginate';

interface FetchData {
    jobs: any[];
    currentPage: number;
    itemsPerPage: number;
    loading: boolean;
}

export class Jobs extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { jobs: [], currentPage: 0, itemsPerPage: 15, loading: true};
    }

    fetchData = (search) => {
        fetch('api/jobs' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ jobs: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Jobs";

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
        let visibleItems = this.state.jobs.slice(this.state.currentPage, this.state.currentPage + this.state.itemsPerPage);

        let jobs = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Jobs.renderJobs(visibleItems);

        return <div className="container-fluid">
            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Jobs</h3>
                        </div>
                        <div className="panel-body">
                            { jobs }

                            { this.state.jobs.length > this.state.itemsPerPage &&
                                <div className="text-center">
                                    <ReactPaginate 
                                        pageCount={Math.ceil(this.state.jobs.length / this.state.itemsPerPage)}
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

    private static renderJobs(jobs) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-2">Job ID</th>
                        <th>Type</th>
                        <th>Priority</th>
                        <th>Task Groups</th>
                        <th>Queued</th>
                        <th>Starting</th>
                        <th>Running</th>
                        <th>Failed</th>
                        <th>Lost</th>
                        <th>Complete</th>
                        <th>Status</th>
                        <th>Submit Time</th>
                    </tr>
                </thead>
                <tbody>
                    {jobs.map(job =>
                        <tr>
                            <td><a href={"/job?id=" + job.ID}>{ job.ID }</a></td>
                            <td>{ job.Type }</td>
                            <td>{ job.Priority }</td>
                            <td>{ job.TotalTaskGroups }</td>
                            <td>{ job.TotalQueuedTasks }</td>
                            <td>{ job.TotalStartingTasks }</td>
                            <td>{ job.TotalRunningTasks }</td>
                            <td>{ job.TotalFailedTasks }</td>
                            <td>{ job.TotalLostTasks }</td>
                            <td>{ job.TotalCompleteTasks }</td>
                            <td>{ job.Status }</td>
                            <td>{ moment(job.SubmitTime).format('DD/MM/YYYY hh:mm:ss A') }</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}
