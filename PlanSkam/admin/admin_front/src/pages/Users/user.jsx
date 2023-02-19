import {Component} from "react";
import Track from "../../components/Track"
import Playlist from "../../components/Playlist";
import axios from "../../services/api/axios";
import {Row, Col, Button, FormControl, Container} from "react-bootstrap";

export default class User extends Component {
    constructor(props) {
        super(props);
        this.state = {
            user: {},
            isAuthor: false,
            tracks: [],
            availableTracks: [],
            availableTracksQuery: '',
            playlists: [],
            availablePlaylists: []
        }
        axios.get(`/users?id=${props.id}`)
            .then(res => {
                console.log(res.data);
                this.setState({user: res.data});
            })
        axios.get(`/users/isAuthor?id=${props.id}`)
            .then(res => {
                console.log(res.data);
                this.setState({isAuthor: res.data === 'true'});
            })
        axios.get(`/users/getFavTracks?id=${props.id}`)
            .then(res => {
                console.log('fav tracks: ' + res.data);
                const tracks = res.data;
                this.setState({tracks: tracks});
                return tracks;
            });
        axios.get(`/playlists/getLikedPlaylists?userId=${props.id}`)
            .then(res => {
                console.log(res.data);
                this.setState({playlists: res.data})
            })
        axios.get(`/playlists/getAvailablePlaylists?userId=${props.id}`)
            .then(res => {
                console.log(res.data);
                this.setState({availablePlaylists: res.data})
            })
        this.changeAuthorState = this.changeAuthorState.bind(this);
        this.changeEmail = this.changeEmail.bind(this);
        this.removeTrackFromFavourites = this.removeTrackFromFavourites.bind(this);
        this.removePlaylistFromLiked = this.removePlaylistFromLiked.bind(this);
        this.addPlaylistToLiked = this.addPlaylistToLiked.bind(this);
        this.addTrackToFavourites = this.addTrackToFavourites.bind(this);
        this.setAvailableTracks = this.setAvailableTracks.bind(this);
    }

    changeAuthorState() {
        const isAuthor = this.state.isAuthor;
        const method = isAuthor ? "makeNotAuthor" : "makeAuthor";
        axios.post(`/users/` + method + `?id=${this.state.user.Id}`)
            .then(res => {
                if (res.status === 201)
                    this.setState({isAuthor: !isAuthor})
            });
    }

    changeEmail() {
        axios.post(`/users/changeEmail?id=${this.state.user.Id}&email=${this.state.user.Email}`)
            .then(res => {
                console.log(res.data)
            })
    }

    removeTrackFromFavourites(trackId) {
        const tracks = this.state.tracks;
        const track = tracks.find(track => track.Id === trackId);
        tracks.splice(tracks.indexOf(track), 1);
        this.setState({tracks: tracks});
        const available = this.state.availableTracks;
        available.push(track);
        this.setState({availableTracks: available});
    }

    addTrackToFavourites(trackId) {
        const available = this.state.availableTracks;
        const track = available.find(track => track.Id === trackId);
        available.splice(available.indexOf(track), 1);
        this.setState({availableTracks: available});
        const fav = this.state.tracks;
        fav.push(track);
        this.setState({tracks: fav});
    }

    setAvailableTracks() {
        const tracks = this.state.tracks;
        axios.get(`/tracks/searchTracks?query=${this.state.availableTracksQuery}`)
            .then(res => {
                console.log('available tracks: ' + res.data);
                this.setState({
                    availableTracks: res.data.filter(track => {
                        let used = false;
                        tracks.forEach(t => {
                            used = used || t.Id === track.Id;
                            return !used;
                        });
                        return !used;
                    })
                })
            })
    }

    removePlaylistFromLiked(playlistId) {
        const playlists = this.state.playlists;
        const playlist = this.state.playlists.find(playlist => playlist.Id === playlistId);
        playlists.splice(playlists.indexOf(playlist), 1);
        this.setState({playlists: playlists});
        const available = this.state.availablePlaylists;
        available.push(playlist);
        this.setState({availablePlaylists: available});
    }

    addPlaylistToLiked(playlistId) {
        const playlists = this.state.playlists;
        const playlist = this.state.availablePlaylists.find(playlist => playlist.Id === playlistId);
        playlists.push(playlist);
        this.setState({playlists: playlists});
        const available = this.state.availablePlaylists;
        available.splice(available.indexOf(playlist), 1);
        this.setState({availablePlaylists: available});
    }

    render() {
        return <Container className="justify-content-center">
            <div className="">
            <Col>
                <Col>
                    <p>Id: {this.state.user.Id}</p>
                </Col>
                <Col>
                    <p>UserName: {this.state.user.UserName}</p>
                </Col>
                <Col>
                    <p>{this.state.isAuthor ? 'Author' : 'Not author'}</p>
                </Col>
            </Col>
            <Button className="ms-3" variant="light" size="sm" onClick={this.changeAuthorState}>
                {this.state.isAuthor ? 'Make not author' : 'Make author'}
            </Button>
            <FormControl className="w-25 mt-3 ms-3" type="text" name="email" value={this.state.user.Email}
                         onChange={e => {
                             const user = this.state.user;
                             user.Email = e.target.value;
                             this.setState({user});
                         }}/>
            <br/>
            <Button className="mb-5 ms-3" variant="light" size="sm" onClick={this.changeEmail}>Change email</Button>
            <h6>Tracks:</h6>
            {this.state.tracks.map(track => {
                return <Track id={track.Id} name={track.Name} userId={this.state.user.Id}
                              delete={this.removeTrackFromFavourites}
                              fav={true}/>;
            })}
            <h6>Available tracks:</h6>
            <FormControl className="w-25 mt-3 ms-3" type="text" name="email" value={this.state.availableTracksQuery}
                         onChange={e => {
                             this.setState({availableTracksQuery: e.target.value});
                         }}/>
            <Button className="mb-5 ms-3" variant="light" size="sm" onClick={this.setAvailableTracks}>Update</Button>
            {this.state.availableTracks.map(track => {
                return <Track id={track.Id} name={track.Name} userId={this.state.user.Id}
                              delete={this.addTrackToFavourites}
                              fav={false}/>;
            })}
            <h6>Liked playlists:</h6>
            {this.state.playlists.map(playlist => {
                return <Playlist id={playlist.Id} name={playlist.Name} userId={this.state.user.Id}
                                 isLiked={true} move={this.removePlaylistFromLiked}/>
            })}
            <h6>Available playlists:</h6>
            {this.state.availablePlaylists.map(playlist => {
                return <Playlist id={playlist.Id} name={playlist.Name} userId={this.state.user.Id}
                                 isLiked={false} move={this.addPlaylistToLiked}/>
            })}
        </div>
        </Container>
    }
}