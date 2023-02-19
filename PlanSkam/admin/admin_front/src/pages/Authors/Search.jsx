import {Component} from "react";
import Author from "../../components/Author";
import axios from "../../services/api/axios";
import {Button, FormControl, Container} from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Track from "../../components/Track";

export default class Search extends Component {
    constructor(props) {
        super(props);
        this.state = {
            query: "",
            authors: []
        }
        this.search = this.search.bind(this);
    }

    search() {
        if (this.state.query !== '')
            axios.get(`/authors/search?query=${this.state.query}`)
                .then(res => {
                    console.log(res.data);
                    this.setState({authors: res.data});
                })
    }
    
    render() {
        return <Container className="justify-content-center">
            <div className="m-lg-5">
                <FormControl className="mb-3 w-50 mt-5" type="text" value={this.state.query} onChange={e => {
                    this.setState({query: e.target.value});
                }}>
                </FormControl>
                <Button variant="light"  onClick={this.search}>search</Button>{' '}
                {this.state.authors.map(author => {
                    return <Author id={author.Id} name={author.Name} delete={this.delete}/>
                })}
            </div>
        </Container>
    }
}