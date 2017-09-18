import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';

export class NavMenu extends React.Component<{}, {}> {
    public render() {
        const pathNames = ["/jobs", "/evaluations", "/allocations", "/clients", "/servers"];

        return <nav className="navbar navbar-default">
            <div className="container-fluid">
                <div className="navbar-header">
                    <button type="button" className="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                        <span className="sr-only">Toggle navigation</span>
                        <span className="icon-bar"></span>
                        <span className="icon-bar"></span>
                        <span className="icon-bar"></span>
                    </button>

                    <a className="navbar-brand" href="/"><img src="/images/nomad-logo.png" alt="Nomad" height="20" /></a>
                </div>

                <div className="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <ul className="nav navbar-nav">
                        <li className={location.pathname === "/" ? "active" : ""}><NavLink to={'/'} exact>Dashboard</NavLink></li>
                        <li className={location.pathname === "/jobs" || location.pathname === "/job" ? "active" : ""}><NavLink to={'/jobs'}>Jobs</NavLink></li>
                        <li className={location.pathname === "/evaluations" || location.pathname === "/evaluation" ? "active" : ""}><NavLink to={'/evaluations'}>Evaluations</NavLink></li>
                        <li className={location.pathname === "/allocations" || location.pathname === "/allocation" ? "active" : ""}><NavLink to={'/allocations'}>Allocations</NavLink></li>
                        <li className={location.pathname === "/clients" || location.pathname === "/client" ? "active" : ""}><NavLink to={'/clients'}>Clients</NavLink></li>
                        <li className={location.pathname === "/servers" || location.pathname === "/server" ? "active" : ""}><NavLink to={'/servers'}>Servers</NavLink></li>
                    </ul>

                    {pathNames.includes(location.pathname) &&
                        <form className="navbar-form navbar-right navbar-search" method="get">
                            <div className="form-group">
                                <input type="text" className="form-control" name="search" placeholder="Search" />
                            </div>

                            <button type="submit" className="btn btn-default"><span className="glyphicon glyphicon-search"></span></button>
                        </form>
                    }
                </div>
            </div>
        </nav>;
    }
}
