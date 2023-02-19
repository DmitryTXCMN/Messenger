import {Component} from "react";
import Track from "../../components/Track";
import {Container, Col, Row} from 'react-bootstrap'
import axios from "../../services/api/axios";

export default class Index extends Component {
    constructor(props) {
        super(props);
        this.state = {
            author: {
                Name: '',
                Tracks: []
            }
        }
        axios.get(`/authors/getWithTracks?id=${props.id}`)
            .then(res => {
                console.log(res.data);
                this.setState({author: res.data});
            })

    }

    render() {
        return <Container>
            <div>
                <h1>{this.state.author.Name}</h1>
                {this.state.author.Tracks.map(track => {
                    return <Track id={track.Id} name={track.Name} delete={this.delete}/>
                })}
            </div>
        </Container>
            
    }
}