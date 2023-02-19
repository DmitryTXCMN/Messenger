import {Component} from "react";
import Track from "../../components/Track";
import axios from "../../services/api/axios";
import {Form, Button, FormControl, Container} from 'react-bootstrap';


export default class Search extends Component {
    constructor(props) {
        super(props);
        this.state = {
            query: "",
            tracks: []
        }
        this.search = this.search.bind(this);
        this.delete = this.delete.bind(this);
    }

    search() {
        if (this.state.query !== '')
            axios.get(`/tracks/searchTracks?query=${this.state.query}`)
                .then(res => {
                    console.log(res.data);
                    this.setState({tracks: res.data});
                })
    }

    delete(trackId) {
        const tracks = this.state.tracks;
        const track = tracks.find(track => track.Id = trackId);
        tracks.splice(tracks.indexOf(track), 1);
        this.setState({tracks: tracks});
    }

    render() {
        return <Container className="justify-content-center">
            <div className="m-lg-5">
                <FormControl className="mb-3 w-50 mt-5" type="text" value={this.state.query} onChange={e => {
                    this.setState({query: e.target.value});
                }}>
                </FormControl>
                <Button variant="light"  onClick={this.search}>search</Button>{' '}
                {this.state.tracks.map(track => {
                    return <Track id={track.Id} name={track.Name} delete={this.delete}/>
                })}
            </div>
        </Container>
    }
}