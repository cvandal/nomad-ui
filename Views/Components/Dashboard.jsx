import React from 'react';
import axios from 'axios';
import {
    defaults,
    Doughnut
} from 'react-chartjs-2';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Dashboard extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            dashboard: null,
            error: null
        }
    }

    componentDidMount() {
        var self = this;

        const makeRequest = () =>
            axios.get("/api/dashboard")
                .then(({ data }) => this.setState({ dashboard: data }))
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
        const { dashboard, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>dashboard</strong> could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (dashboard === null) {
            return null
        }

        return (
            <div>
                <div className="row">
                    <div className="col-sm-3">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Job States</h3>
                            </div>
                            <div className="panel-body">
                                <Doughnut
                                    data={{
                                        labels: ['Running', 'Pending', 'Dead'],
                                        datasets: [{
                                            data: [dashboard.runningJobs, dashboard.pendingJobs, dashboard.deadJobs],
                                            backgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            hoverBackgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            borderColor: ['#1e1e1e', '#1e1e1e', '#1e1e1e']
                                        }]
                                    }}
                                    options={{
                                        cutoutPercentage: 75,
                                        legend: {
                                            position: 'left',
                                            labels: {
                                                fontColor: "#fff"
                                            }
                                        },
                                        animation: {
                                            animateScale: false,
                                            animateRotate: false
                                        }
                                    }}
                                    height={ 200 }
                                />
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-3">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Allocation States</h3>
                            </div>
                            <div className="panel-body">
                                <Doughnut
                                    data={{
                                        labels: ['Running', 'Pending', 'Dead'],
                                        datasets: [{
                                            data: [dashboard.runningAllocations, dashboard.pendingAllocations, dashboard.deadAllocations],
                                            backgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            hoverBackgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            borderColor: ['#1e1e1e', '#1e1e1e', '#1e1e1e']
                                        }]
                                    }}
                                    options={{
                                        cutoutPercentage: 75,
                                        legend: {
                                            position: 'left',
                                            labels: {
                                                fontColor: "#fff"
                                            }
                                        },
                                        animation: {
                                            animateScale: false,
                                            animateRotate: false
                                        }
                                    }}
                                    height={ 200 }
                                />
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-3">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Client States</h3>
                            </div>
                            <div className="panel-body">
                                <Doughnut
                                    data={{
                                        labels: ['Up', 'Draining', 'Down'],
                                        datasets: [{
                                            data: [dashboard.upClients, dashboard.drainingClients, dashboard.downClients],
                                            backgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            hoverBackgroundColor: ['#1fb58f', '#eab126', '#f24c4e'],
                                            borderColor: ['#1e1e1e', '#1e1e1e', '#1e1e1e']
                                        }]
                                    }}
                                    options={{
                                        cutoutPercentage: 75,
                                        legend: {
                                            position: 'left',
                                            labels: {
                                                fontColor: "#fff"
                                            }
                                        },
                                        animation: {
                                            animateScale: false,
                                            animateRotate: false
                                        }
                                    }}
                                    height={ 200 }
                                />
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-3">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Server States</h3>
                            </div>
                            <div className="panel-body">
                                <Doughnut
                                    data={{
                                        labels: ['Up', 'Down'],
                                        datasets: [{
                                            data: [dashboard.upServers, dashboard.downServers],
                                            backgroundColor: ['#1fb58f', '#f24c4e'],
                                            hoverBackgroundColor: ['#1fb58f', '#f24c4e'],
                                            borderColor: ['#1e1e1e', '#1e1e1e']
                                        }]
                                    }}
                                    options={{
                                        cutoutPercentage: 75,
                                        legend: {
                                            position: 'left',
                                            labels: {
                                                fontColor: "#fff"
                                            }
                                        },
                                        animation: {
                                            animateScale: false,
                                            animateRotate: false
                                        }
                                    }}
                                    height={ 200 }
                                />
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col-sm-12">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Allocation Events</h3>
                            </div>
                            <div className="panel-body">
                                <div className="table-responsive">
                                    <table className="table table-hover">
                                        <thead>
                                            <tr>
                                                <th className="col-sm-3">Name</th>
                                                <th className="col-sm-1">Type</th>
                                                <th className="col-sm-6">Message</th>
                                                <th className="col-sm-2">Time</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {dashboard.events.map((event) => {
                                                return (
                                                    <tr>
                                                        <td><a href={"/allocation?id=" + event.allocationID}>{event.allocationName}</a></td>
                                                        <td>{event.type}</td>
                                                        <td>
                                                            {event.restartReason}
                                                            {event.setupError}
                                                            {event.driverError}
                                                            {event.message}
                                                            {event.killError}
                                                            {event.killReason}
                                                            {event.downloadError}
                                                            {event.validationError}
                                                            {event.failedSibling}
                                                            {event.vaultError}
                                                            {event.taskSignalReason}
                                                            {event.taskSignal}
                                                            {event.driverMessage}
                                                        </td>
                                                        <td>{moment(event.dateTime).format('DD/MM/YYYY hh:mm:ss A')}</td>
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
