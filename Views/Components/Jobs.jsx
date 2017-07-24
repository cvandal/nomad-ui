import React from 'react';
import axios from 'axios';
import Pager from 'react-pager';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Jobs extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            jobs: null,
            error: null,
            current: 0
        }

        this.handlePageChanged = this.handlePageChanged.bind(this);
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () => {
            this.props.location.search ?
                axios.get("/api/jobs" + this.props.location.search)
                    .then(({ data }) => this.setState({ jobs: data }))
                    .catch((error) => this.setState({ error: error })) :
                axios.get("/api/jobs")
                    .then(({ data }) => this.setState({ jobs: data }))
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
        const { jobs, error, current } = this.state;
        const itemsPerPage = 15;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the list of <strong>jobs</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (jobs === null) {
            return null
        }

        var visibleItems = jobs.slice(itemsPerPage * current, (itemsPerPage * current) + itemsPerPage);

        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Jobs</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-2">ID</th>
                                                <th className="col-sm-1">Type</th>
                                                <th className="col-sm-1">Priority</th>
                                                <th className="col-sm-1">Task Groups</th>
                                                <th className="col-sm-1">Queued</th>
                                                <th className="col-sm-1">Starting</th>
                                                <th className="col-sm-1">Running</th>
                                                <th className="col-sm-1">Failed</th>
                                                <th className="col-sm-1">Lost</th>
                                                <th className="col-sm-1">Complete</th>
                                                <th className="col-sm-1">Status</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {visibleItems.map((job) => {
                                                return (
                                                    <tr>
                                                        <td><a href={"/job?id=" + job.id}>{job.id}</a></td>
                                                        <td>{job.type}</td>
                                                        <td>{job.priority}</td>
                                                        <td>{job.numOfTaskGroups}</td>
                                                        <td>{job.queuedTaskGroups}</td>
                                                        <td>{job.startingTaskGroups}</td>
                                                        <td>{job.runningTaskGroups}</td>
                                                        <td>{job.failedTaskGroups}</td>
                                                        <td>{job.lostTaskGroups}</td>
                                                        <td>{job.completeTaskGroups}</td>
                                                        <td>{job.status}</td>
                                                    </tr>
                                                )
                                            })}
                                        </tbody>
                                    </table>
                                </div>

                                {jobs.length > 15 ?
                                    <div className="text-center">
                                        <Pager
                                            total={Math.ceil(jobs.length / itemsPerPage)}
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
