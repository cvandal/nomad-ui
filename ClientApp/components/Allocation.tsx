import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Doughnut } from 'react-chartjs-2';
import moment from 'moment';
import $ from 'jquery';

interface FetchData {
    allocation: any;
    log: any;
    loading: boolean;
}

export class Allocation extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { allocation: null, log: null, loading: true};
    }

    fetchData = (search) => {
        fetch('api/allocation' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ allocation: data, loading: false });
            });
    }

    fetchLog = (client, id, log) => {
        fetch('api/allocation/log?client=' + client + '&id=' + id + '&log=' + log)
            .then(response => response.text())
            .then(data => {
                this.setState({ log: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Allocation";

        this.fetchData(this.props.location.search);
        this.poll = setInterval(() => this.fetchData(this.props.location.search), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.poll);
    }

    public render() {
        let allocation = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : this.renderAllocation(this.state.allocation);
        
        return <div className="container-fluid">
            { allocation }
        </div>;
    }

    public renderAllocation(allocation) {
        let activeCpu = allocation.Stats.ResourceUsage.CpuStats.Percent;
        let idleCpu = (100 - activeCpu);
        let consumedMem = ((allocation.Stats.ResourceUsage.MemoryStats.MaxUsage / 1024) / 1024);
        let availableMem = (allocation.Resources.MemoryMB - (allocation.Stats.ResourceUsage.MemoryStats.MaxUsage / 1024) / 1024);
        let ipAddress = allocation.Resources.Networks[0].IP;
        let dynamicPort = allocation.Resources.Networks[0].DynamicPorts ? ':' + allocation.Resources.Networks[0].DynamicPorts[0].Value : "";
        
        $(document).ready(function() {
            var divHeight = $('.test1').height();
            console.log(divHeight);
            divHeight = divHeight-96;
            $('.test2').css('max-height', divHeight+'px');
        });

        return <div>
            <div className="row">
                <div className="col-sm-6 col-parent test1">
                    <div className="row">
                        <div className="col-sm-12 col-parent">
                            <div className="row">
                                <div className="col-sm-6">
                                    <div className="panel panel-default">
                                        <div className="panel-heading">
                                            <h3 className="panel-title text-center">Allocation Properties</h3>
                                        </div>
                                        <div className="panel-body">
                                            <ul className="list-group">
                                                <li className="list-group-item"><strong>ID:</strong> { allocation.ID }</li>
                                                <li className="list-group-item"><strong>Name:</strong> { allocation.Name }</li>
                                                <li className="list-group-item"><strong>Task Group:</strong> { allocation.TaskGroup }</li>
                                                <li className="list-group-item"><strong>Desired Status:</strong> { allocation.DesiredStatus }</li>
                                                <li className="list-group-item"><strong>Job ID:</strong> <a href={"/job?id=" + allocation.JobID}>{ allocation.JobID }</a></li>
                                                <li className="list-group-item"><strong>Evaluation ID:</strong> <a href={"/evaluation?id=" + allocation.EvalID}>{ allocation.EvalID }</a></li>
                                                <li className="list-group-item"><strong>Client ID:</strong> <a href={"/client?id=" + allocation.NodeID}>{ allocation.NodeID }</a></li>
                                                <li className="list-group-item"><strong>Client Description:</strong> { allocation.ClientDescription ? allocation.ClientDescription : "N/A" }</li>
                                                <li className="list-group-item"><strong>Client Status:</strong> { allocation.ClientStatus }</li>
                                                <li className="list-group-item"><strong>Create Time:</strong> { moment(allocation.CreateTime).format('DD/MM/YYYY hh:mm:ss A') }</li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>

                                <div className="col-sm-6">
                                    <div className="panel panel-default">
                                        <div className="panel-heading">
                                            <h3 className="panel-title text-center">Resources</h3>
                                        </div>
                                        <div className="panel-body">
                                            <ul className="list-group">
                                                <li className="list-group-item"><strong>CPU (Mhz):</strong> { allocation.Resources.CPU }</li>
                                                <li className="list-group-item"><strong>Memory (MB):</strong> { allocation.Resources.MemoryMB }</li>
                                                <li className="list-group-item"><strong>Disk (MB):</strong> { allocation.Resources.DiskMB }</li>
                                                <li className="list-group-item"><strong>Network (Mbps):</strong> { allocation.Resources.Networks[0].MBits }</li>
                                                <li className="list-group-item"><strong>Address:</strong> <a href={"http://" + ipAddress + dynamicPort} target="_blank">http://{ipAddress}{dynamicPort}</a></li>
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
                                                height={ 200 }
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
                                                    labels: ['Available  MB', 'Consumed MB'],
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
                                                height={ 200 }
                                            />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="col-sm-6">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Logs</h3>
                        </div>
                        <div className="panel-body">
                            <div className="row">
                                <div className="col-sm-3 col-parent">
                                    <ul className="list-group">
                                        {allocation.Logs.sort().map(log =>
                                            <li className="list-group-item"><button className="btn btn-link" onClick={e => this.fetchLog(allocation.Resources.Networks[0].IP, allocation.ID, log.Name)}>{ log.Name }, { log.Size } (Bytes)</button></li>
                                        )}
                                    </ul>
                                </div>
                                <div className="col-sm-9 col-parent">
                                    <pre className="log test2">
                                        { this.state.log }
                                    </pre>
                                </div>
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
                                        {Object.entries(allocation.TaskStates).map(([key]) => allocation.TaskStates[key].Events.sort((a, b) => a.Time < b.Time).map(event =>
                                            <tr>
                                                <td>{ key }</td>
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
                                        )).reduce((flattened, rows) => [...flattened, ...rows], [])}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>;
    }
}
