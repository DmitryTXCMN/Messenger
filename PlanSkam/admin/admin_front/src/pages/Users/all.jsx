import {Component} from "react";
import {NavLink} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import {Col, Container, Row, Table} from "react-bootstrap";
import Button from 'react-bootstrap/Button';
import axios from "../../services/api/axios";

export default class All extends Component {
    constructor(props) {
        super(props);

        this.state = {
            users: []
        }
        this.click = this.click.bind(this);
    }

    click() {
        axios.get("/users/getAll")
            .then(res => {
                if (res.status === 200) this.setState({users: res.data});
            });
    }

    render() {
        const renderUsers = (users) => {
            console.log(users);
            return (
                <Container>
                    <div className="d-flex justify-content-center align-items-center py-3">
                        <Table striped bordered hover>
                            <thead  >
                            <Row className='ml-10'>
                                <Col>Id</Col>
                                <Col>UserName</Col>
                                <Col></Col>
                            </Row>
                            </thead>
                            <tbody>
                            <Container>
                                {
                                    users.map(user => {
                                        return <Row className="nav nav-pills">
                                            <Col>{user.Id}</Col>
                                            <Col>{user.UserName}</Col>
                                            <Col>
                                                <Button variant="light" className="mb-5 ms-3" size="sm"><NavLink className="text-decoration-none text-black" to={`../user/${user.Id}`}>open</NavLink></Button>
                                            </Col>
                                        </Row>
                                    })
                                }
                            </Container>
                            </tbody>
                        </Table>
                    </div>
                </Container>)

        }


        this.click();


        return <Container>
            <div>
                {renderUsers(this.state.users)}
                <Button variant="light" onClick={this.click}>Update</Button>{' '}
            </div>
        </Container>
    }
}
