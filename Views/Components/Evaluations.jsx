import React from 'react';
import axios from 'axios';
import Pager from 'react-pager';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Evaluations extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            evaluations: null,
            error: null,
            current: 0
        }

        this.handlePageChanged = this.handlePageChanged.bind(this);
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () => {
            this.props.location.search ?
                axios.get("/api/evaluations" + this.props.location.search)
                    .then(({ data }) => this.setState({ evaluations: data }))
                    .catch((error) => this.setState({ error: error })) :
                axios.get("/api/evaluations")
                    .then(({ data }) => this.setState({ evaluations: data }))
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
        const { evaluations, error, current } = this.state;
        const itemsPerPage = 15;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the list of <strong>evaluations</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (evaluations === null) {
            return null
        }

        var visibleItems = evaluations.slice(itemsPerPage * current, (itemsPerPage * current) + itemsPerPage);

        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Evaluations</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
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
                                            {visibleItems.map((evaluation) => {
                                                return (
                                                    <tr>
                                                        <td><a href={"/evaluation?id=" + evaluation.id}>{evaluation.id}</a></td>
                                                        <td><a href={"/job?id=" + evaluation.jobID}>{evaluation.jobID}</a></td>
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

                                {evaluations.length > 15 ?
                                    <div className="text-center">
                                        <Pager
                                            total={Math.ceil(evaluations.length / itemsPerPage)}
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
