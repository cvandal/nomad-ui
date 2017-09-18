import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Doughnut } from 'react-chartjs-2';
import moment from 'moment';

interface FetchData {
    jobStatus: any;
    allocationStatus: any;
    clientStatus: any;
    serverStatus: any;
    events: any[];
    loading: boolean;
}

export class Dashboard extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { jobStatus: null, allocationStatus: null, clientStatus: null, serverStatus: null, events: [], loading: true };
    }

    fetchData() {
        fetch('api/jobs/status')
            .then(response => response.json() as Promise<any>)
            .then(data => {
                this.setState({ jobStatus: data, loading: false });
            });
        fetch('api/allocations/status')
            .then(response => response.json() as Promise<any>)
            .then(data => {
                this.setState({ allocationStatus: data, loading: false });
            });
        fetch('api/clients/status')
            .then(response => response.json() as Promise<any>)
            .then(data => {
                this.setState({ clientStatus: data, loading: false });
            });
        fetch('api/servers/status')
            .then(response => response.json() as Promise<any>)
            .then(data => {
                this.setState({ serverStatus: data, loading: false });
            });
        fetch('api/allocations/events?count=15')
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ events: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Dashboard";

        this.fetchData();
        this.poll = setInterval(() => this.fetchData(), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.poll);
    }

    public render() {
        let jobStatusChart = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Dashboard.renderJobStatusChart(this.state.jobStatus);

        let allocationStatusChart = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Dashboard.renderAllocationStatusChart(this.state.allocationStatus);

        let clientStatusChart = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Dashboard.renderClientStatusChart(this.state.clientStatus);

        let serverStatusChart = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Dashboard.renderServerStatusChart(this.state.serverStatus);

        let allocationEventsTable = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Dashboard.renderAllocationsEventsTable(this.state.events);

        return <div className="container-fluid">
            <div className="row">
                <div className="col-sm-3">
                    { jobStatusChart }
                </div>

                <div className="col-sm-3">
                    { allocationStatusChart }
                </div>

                <div className="col-sm-3">
                    { clientStatusChart }
                </div>

                <div className="col-sm-3">
                    { serverStatusChart }
                </div>
            </div>

            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Allocation Events</h3>
                        </div>
                        <div className="panel-body">
                            { allocationEventsTable }
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }

    private static renderJobStatusChart(jobStatus) {
        return <div className="panel panel-default">
            <div className="panel-heading">
                <h3 className="panel-title text-center">Job Status</h3>
            </div>
            <div className="panel-body">
                <Doughnut
                    data={{
                        labels: ['Running', 'Pending', 'Dead'],
                        datasets: [{
                            data: [jobStatus.Running, jobStatus.Pending, jobStatus.Dead],
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
        </div>;
    }

    private static renderAllocationStatusChart(allocationStatus) {
        return <div className="panel panel-default">
            <div className="panel-heading">
                <h3 className="panel-title text-center">Allocation Status</h3>
            </div>
            <div className="panel-body">
                <Doughnut
                    data={{
                        labels: ['Running', 'Pending', 'Dead'],
                        datasets: [{
                            data: [allocationStatus.Running, allocationStatus.Pending, allocationStatus.Dead],
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
        </div>;
    }

    private static renderClientStatusChart(clientStatus) {
        return <div className="panel panel-default">
            <div className="panel-heading">
                <h3 className="panel-title text-center">Client Status</h3>
            </div>
            <div className="panel-body">
                <Doughnut
                    data={{
                        labels: ['Up', 'Draining', 'Down'],
                        datasets: [{
                            data: [clientStatus.Up, clientStatus.Draining, clientStatus.Down],
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
        </div>;
    }

    private static renderServerStatusChart(serverStatus) {
        return <div className="panel panel-default">
            <div className="panel-heading">
                <h3 className="panel-title text-center">Server Status</h3>
            </div>
            <div className="panel-body">
                <Doughnut
                    data={{
                        labels: ['Up', 'Down'],
                        datasets: [{
                            data: [serverStatus.Up, serverStatus.Down],
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
        </div>;
    }

    private static renderAllocationsEventsTable(events) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-3">Name</th>
                        <th className="col-sm-2">Type</th>
                        <th className="col-sm-5">Message</th>
                        <th className="col-sm-2">Time</th>
                    </tr>
                </thead>
                <tbody>
                    {events.map(event =>
                        <tr>
                            <td><a href={"/allocation?id=" + event.ID}>{ event.Name }</a></td>
                            <td>{ event.Type }</td>
                            <td>
                                { event.Message }
                                { event.RestartReason }
                                { event.SetupError }
                                { event.DriverError }
                                { event.KillError }
                                { event.KillReason }
                                { event.DownloadError }
                                { event.ValidationError }
                                { event.FailedSibling }
                                { event.VaultError }
                                { event.TaskSignalReason }
                                { event.TaskSignal }
                                { event.DriverMessage }
                                { event.GenericSource }
                            </td>
                            <td>{ moment(event.Time).format('DD/MM/YYYY hh:mm:ss A') }</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}
