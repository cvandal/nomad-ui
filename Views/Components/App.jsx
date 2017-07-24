import React from 'react';
import {
    BrowserRouter as Router,
    Route,
    Link
} from 'react-router-dom';
import Dashboard from './Dashboard.jsx';
import Jobs from './Jobs.jsx';
import Job from './Job.jsx';
import Evaluations from './Evaluations.jsx';
import Evaluation from './Evaluation.jsx';
import Allocations from './Allocations.jsx';
import Allocation from './Allocation.jsx';
import Clients from './Clients.jsx';
import Client from './Client.jsx';
import Servers from './Servers.jsx';
import Server from './Server.jsx';

export default class App extends React.Component {
    render() {
        return (
            <Router>
                <div>
                    <Route exact path="/" component={Dashboard} />
                    <Route exact path="/jobs" component={Jobs} />
                    <Route path="/job" component={Job} />
                    <Route exact path="/evaluations" component={Evaluations} />
                    <Route path="/evaluation" component={Evaluation} />
                    <Route exact path="/allocations" component={Allocations} />
                    <Route path="/allocation" component={Allocation} />
                    <Route exact path="/clients" component={Clients} />
                    <Route path="/client" component={Client} />
                    <Route exact path="/servers" component={Servers} />
                    <Route path="/server" component={Server} />
                </div>
            </Router>
        );
    }
}
