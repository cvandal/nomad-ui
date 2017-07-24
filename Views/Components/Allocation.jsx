import React from 'react';
import axios from 'axios';
import {
    defaults,
    Doughnut
} from 'react-chartjs-2';
import moment from 'moment';

global.jQuery = require('jquery');
var bootstrap = require('bootstrap');

export default class Allocation extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            allocation: null,
            error: null,
            currentLog: {
                loading: false,
                contents: ''
            }
        }
    }

    componentDidMount() {
        const makeRequest = () =>
            axios.get("/api/allocation" + this.props.location.search)
                .then(({ data }) => this.setState({ allocation: data }))
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

    handleLogClick(logName) {
        const { allocation: { id, resources: { networks } } } = this.state;
        const { ip } = networks[0]

        return e => {
            e.preventDefault()

            this.setState({
                currentLog: {
                    loading: true,
                    contents: ''
                }
            })

            fetch(`/allocation/log?client=${ip}&id=${id}&log=${logName}`)
                .then(res => res.text())
                .then(contents => this.setState({
                    currentLog: {
                        loading: false,
                        contents,
                    }
                }))
        }
    }

    render() {
        const { allocation, error } = this.state;

        if (error) {
            return (
                <div className="container-fluid">
                    <div className="text-center">
                        <h1>Oops!</h1>
                        <p>Something went wrong and the <strong>allocation</strong> you've selected could not be loaded.</p>
                    </div>
                </div>
            )
        }

        if (allocation === null) {
            return null
        }

        var idleCpu = (100 - allocation.stats.resourceUsage.cpuStats.percent);
        var activeCpu = allocation.stats.resourceUsage.cpuStats.percent;
        var availableMem = (allocation.resources.memoryMB - (allocation.stats.resourceUsage.memoryStats.maxUsage / 1024) / 1024);
        var consumedMem = ((allocation.stats.resourceUsage.memoryStats.maxUsage / 1024) / 1024);

        return (
            <div>
                <div className="row">
                    <div className="col-sm-6">
                        <div className="row row-condensed">
                            <div className="col-sm-6">
                                <div className="panel panel-default">
                                    <div className="panel-heading">
                                        <h3 className="panel-title text-center">Allocation Properties</h3>
                                    </div>
                                    <div className="panel-body">
                                        <ul className="list-group">
                                            <li className="list-group-item"><strong>ID:</strong> {allocation.id}</li>
                                            <li className="list-group-item"><strong>Name:</strong> {allocation.name}</li>
                                            <li className="list-group-item"><strong>Task Group:</strong> {allocation.taskGroup}</li>
                                            <li className="list-group-item"><strong>Desired Status:</strong> {allocation.desiredStatus}</li>
                                            <li className="list-group-item"><strong>Job:</strong> <a href={"/job?id=" + allocation.jobID}>{allocation.jobID}</a></li>
                                            <li className="list-group-item"><strong>Evaluation:</strong> <a href={"/evaluation?id=" + allocation.evalID}>{allocation.evalID}</a></li>
                                            <li className="list-group-item"><strong>Client:</strong> <a href={"/client?id=" + allocation.nodeID}>{allocation.nodeID}</a></li>
                                            <li className="list-group-item"><strong>Client Description:</strong> {allocation.clientDescription}</li>
                                            <li className="list-group-item"><strong>Client Status:</strong> {allocation.clientStatus}</li>
                                            <li className="list-group-item"><strong>Create Time:</strong> {moment(allocation.createDateTime).format('DD/MM/YYYY hh:mm:ss A')}</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div className="col-sm-6">
                                <div className="panel panel-default">
                                    <div className="panel-heading">
                                        <h3 className="panel-title text-center">Allocation Resources</h3>
                                    </div>
                                    <div className="panel-body">
                                        <ul className="list-group">
                                            <li className="list-group-item"><strong>CPU (MHz):</strong> {allocation.resources.cpu}</li>
                                            <li className="list-group-item"><strong>Memory (MB):</strong> {allocation.resources.memoryMB}</li>
                                            <li className="list-group-item"><strong>Disk (MB):</strong> {allocation.resources.diskMB}</li>
                                            <li className="list-group-item">
                                                <strong>Network (Mpbs):</strong>
                                                <ul className="list-group list-group-condensed">
                                                    {allocation.resources.networks.map((network) => {
                                                        return (
                                                            <li className="list-group-item list-group-item-condensed">{network.mBits}</li>
                                                        )
                                                    })}
                                                </ul>
                                            </li>
                                            <li className="list-group-item">
                                                <strong>Address:</strong>
                                                {allocation.resources.networks.map((network) => {
                                                    return ([
                                                        <ul className="list-group list-group-condensed">
                                                            {network.dynamicPorts.map((port) => {
                                                                return (
                                                                    <li className="list-group-item list-group-item-condensed"><a href={"http://" + network.ip + ":" + port.value} target="_blank">http://{network.ip}:{port.value}</a></li>
                                                                )
                                                            })}
                                                        </ul>
                                                    ])
                                                })}
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-sm-6">
                                <div className="panel panel-default">
                                    <div className="panel-heading">
                                        <h3 className="panel-title text-center">CPU Utilisation</h3>
                                    </div>
                                    <div className="panel-body">
                                        <Doughnut
                                            data={{
                                                labels: ['Idle %', 'Active %'],
                                                datasets: [{
                                                    data: [idleCpu, activeCpu],
                                                    backgroundColor: ['#1fb58f', '#eab126'],
                                                    hoverBackgroundColor: ['#1fb58f', '#eab126'],
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
                                            height={200}
                                        />
                                    </div>
                                </div>
                            </div>

                            <div className="col-sm-6">
                                <div className="panel panel-default">
                                    <div className="panel-heading">
                                        <h3 className="panel-title text-center">Memory Utilisation</h3>
                                    </div>
                                    <div className="panel-body">
                                        <Doughnut
                                            data={{
                                                labels: ['Available MB', 'Consumed MB'],
                                                datasets: [{
                                                    data: [availableMem, consumedMem],
                                                    backgroundColor: ['#1fb58f', '#eab126'],
                                                    hoverBackgroundColor: ['#1fb58f', '#eab126'],
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
                                            height={200}
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="col-sm-6">
                        <div className="panel panel-default">
                            <div className="panel-heading">
                                <h3 className="panel-title text-center">Allocation Logs</h3>
                            </div>
                            <div className="panel-body">
                                <div className="col-sm-4">
                                    <ul className="list-group">
                                        {allocation.logs.map((log) => {
                                            return (
                                                <li className="list-group-item"><a className="log" href="#" onClick={this.handleLogClick(log.name)}>{log.name}</a>, {log.size} (Bytes)</li>
                                            )
                                        })}
                                    </ul>
                                </div>

                                <div className="col-sm-8">
                                    {this.state.currentLog.loading && <pre className="log-content">Loading...</pre>}

                                    <pre className="log-content">
                                        {!this.state.currentLog.loading && this.state.currentLog.contents}
                                    </pre>
                                </div>
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
                                            {Object.entries(allocation.taskStates).map(([key]) => allocation.taskStates[key].events.sort((a, b) => a.dateTime < b.dateTime).map((event) => (
                                                <tr>
                                                    <td>{allocation.name}</td>
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
                                            ))).reduce((flattened, rows) => [...flattened, ...rows], [])}
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
