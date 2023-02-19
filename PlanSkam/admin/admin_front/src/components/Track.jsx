import axios from "../services/api/axios";
import {Component, Input} from "react";
import {Button, InputGroup, FormControl, Row, Col, Container} from "react-bootstrap";

export default class Track extends Component {
    constructor(props) {
        super(props);
        this.state = {
            name: props.name
        };
        this.removeFromFavourites = this.removeFromFavourites.bind(this);
        this.addToFavourites = this.addToFavourites.bind(this);
        this.delete = this.delete.bind(this);
        this.changeName = this.changeName.bind(this);
    }

    removeFromFavourites() {
        axios.post(`/users/removeTrackFromFavourites?userId=${this.props.userId}&trackId=${this.props.id}`)
            .then(res => {
                console.log(res.data);
                this.props.delete(this.props.id);
            });
    }

    addToFavourites() {
        axios.post(`/users/addTrackToFavourites?userId=${this.props.userId}&trackId=${this.props.id}`)
            .then(res => {
                console.log(res.data);
                this.props.delete(this.props.id);
            });
    }

    delete() {
        axios.post(`/tracks/removeTrack?id=${this.props.id}`)
            .then(res => {
                if (res.status === 201) {
                    console.log(res.data);
                    this.props.delete(this.props.id);
                }
            })
    }

    changeName() {
        axios.post(`/tracks/changeName?id=${this.props.id}&name=${this.state.name}`)
            .then(res => {
                if (res.status === 201) {
                    console.log(res.data);
                }
            });
    }

    render() {
            return <Container className="justify-content-md-center">
                <div className="track w-75 m-3 border-0 border-bottom ">
            <Row xs="auto">
                <Col><p>Id: {this.props.id}</p></Col>
                <Col><FormControl className="mb-3" type="text" value={this.state.name} onChange={e => {
                    this.setState({name: e.target.value});}}>
                </FormControl></Col>
                <Col className="me-5"><Button variant="light" onClick={this.changeName} size="sm">Change</Button>{' '}</Col>
                <Col className="ms-5">{this.props.userId != null ?
                    this.props.fav ?
                        <Button variant="light" size="sm" onClick={this.removeFromFavourites}>remove from favourites</Button> :
                        <Button variant="light" size="sm" onClick={this.addToFavourites}>add to favourites</Button> :
                    null}</Col>
                <Col ><Button variant="light" size="sm" onClick={this.delete}>delete</Button>{' '}</Col>
            </Row>
        </div>
        </Container>
    }
}