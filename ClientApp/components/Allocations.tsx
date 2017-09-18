import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import moment from 'moment';
import ReactPaginate from 'react-paginate';

interface FetchData {
    allocations: any[];
    currentPage: number;
    itemsPerPage: number;
    loading: boolean;
}

export class Allocations extends React.Component<RouteComponentProps<{}>, FetchData> {
    poll: number;

    constructor() {
        super();

        this.state = { allocations: [], currentPage: 0, itemsPerPage: 15, loading: true};
    }

    fetchData = (search) => {
        fetch('api/allocations' + search)
            .then(response => response.json() as Promise<any[]>)
            .then(data => {
                this.setState({ allocations: data, loading: false });
            });
    }

    componentDidMount() {
        document.title = "Nomad - Allocations";

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
        let visibleItems = this.state.allocations.slice(this.state.currentPage, this.state.currentPage + this.state.itemsPerPage);

        let allocations = this.state.loading
            ? <p className="text-center"><em>Loading...</em></p>
            : Allocations.renderAllocations(visibleItems);

        return <div className="container-fluid">
            <div className="row">
                <div className="col-sm-12">
                    <div className="panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title text-center">Allocations</h3>
                        </div>
                        <div className="panel-body">
                            { allocations }

                            { this.state.allocations.length > this.state.itemsPerPage &&
                                <div className="text-center">
                                    <ReactPaginate 
                                        pageCount={Math.ceil(this.state.allocations.length / this.state.itemsPerPage)}
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

    private static renderAllocations(allocations) {
        return <div className="table-responsive">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th className="col-sm-2">ID</th>
                        <th className="col-sm-2">Name</th>
                        <th className="col-sm-2">Task Group</th>
                        <th className="col-sm-2">Desired Status</th>
                        <th className="col-sm-2">Client Status</th>
                        <th className="col-sm-2">Create Time</th>
                    </tr>
                </thead>
                <tbody>
                    {allocations.map(allocation =>
                        <tr>
                            <td>{ allocation.DesiredStatus === "run" ? <a href={"/allocation?id=" + allocation.ID}>{allocation.ID}</a> : allocation.ID }</td>
                            <td>{ allocation.Name }</td>
                            <td>{ allocation.TaskGroup }</td>
                            <td>{ allocation.DesiredStatus }</td>
                            <td>{ allocation.ClientStatus }</td>
                            <td>{ moment(allocation.CreateTime).format('DD/MM/YYYY hh:mm:ss A') }</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}
