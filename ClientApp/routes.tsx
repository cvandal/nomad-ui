import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Dashboard } from './components/Dashboard';
import { Jobs } from './components/Jobs';
import { Job } from './components/Job';
import { Evaluations } from './components/Evaluations';
import { Evaluation } from './components/Evaluation';
import { Allocations } from './components/Allocations';
import { Allocation } from './components/Allocation';

export const routes = <Layout>
    <Route exact path='/' component={Dashboard} />
    <Route exact path='/jobs' component={Jobs} />
    <Route path="/job" component={Job} />
    <Route exact path='/evaluations' component={Evaluations} />
    <Route path='/evaluation' component={Evaluation} />
    <Route exact path='/allocations' component={Allocations} />
    <Route path='/allocation' component={Allocation} />
</Layout>;
