import {Component} from "react";
import {Button, Col, Row, FormControl, Container } from "react-bootstrap";
import axios from "../services/api/axios";

export default class Playlist extends Component {
    constructor(props) {
        super(props);
        this.removePlaylistFromLiked = this.removePlaylistFromLiked.bind(this);
        this.addPlaylistToLiked = this.addPlaylistToLiked.bind(this);
    }

    removePlaylistFromLiked() {
        axios.post(`/users/removePlaylistFromLiked?userId=${this.props.userId}&playlistId=${this.props.id}`)
            .then(res => {
                if (res.status === 201) {
                    console.log(res.data);
                    this.props.move(this.props.id);
                }
            });
    }

    addPlaylistToLiked() {
        axios.post(`/users/addPlaylistToLiked?userId=${this.props.userId}&playlistId=${this.props.id}`)
            .then(res => {
                if (res.status === 201) {
                    console.log(res.data);
                    this.props.move(this.props.id);
                }
            });
    }

    render() {
        return <Container className="justify-content-center">
            <div className="playlist w-50 m-3 border-0 border-bottom">
                <Row className="h-50">
                    <Col > <p> {this.props.id} {this.props.name} </p>
                    </Col>
                    <Col ><Button className="mb-5 ms-3" variant="light"  size="sm" onClick={this.props.isLiked ? this.removePlaylistFromLiked : this.addPlaylistToLiked}>{this.props.isLiked ? "remove" : "add"}</Button>{' '}</Col>
                </Row>
            </div>
        </Container>
    }
}