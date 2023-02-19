import {Component} from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import Form from 'react-bootstrap/Form';
import {Button, Col, Container, Row} from 'react-bootstrap';
import {NavLink} from "react-router-dom";
import axios from "../services/api/axios";

export default class Author extends Component {
    constructor(props) {
        super(props);
        this.state = {
            name: props.name
        }
        this.changeName = this.changeName.bind(this);
    }

    changeName(){
        axios.post(`/authors/changeName?id=${this.props.id}&name=${this.state.name}`)
            .then(res =>{
                console.log(res.data);
            });
    }
    
    render() {
        return <Container className="justify-content-center">
            <div className="author w-100 m-3 border-0 border-bottom">
                <Row className="h-50">
                    <Col > <p> {this.props.id}  </p>
                    </Col>
                    <Col>
                        Name: <input type="text" value={this.state.name} onChange={e => {
                        this.setState({name: e.target.value});
                    }}/>
                    </Col>
                    <Col>
                        <Button variant="light" className="mb-5 ms-3" size="sm" onClick={this.changeName}>Change name</Button>{' '}
                    </Col>
                    <Col>
                        <Button variant="light" className="mb-5 ms-3" size="sm"><NavLink className="text-decoration-none text-black" to={`../author/${this.props.id}`}>Open</NavLink></Button>
                    </Col>
                </Row>
            </div>
        </Container>
    }
}