import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    makeStyles,
} from "@material-ui/core";
import {Link, useNavigate} from "react-router-dom";
import "./App.css";
import Routes from "./routes/Routes";
import useAuth from "./hooks/useAuth";

const useStyles = makeStyles((theme) => ({
    root: {
        flexGrow: 1,
    },
    rightToolbar: {
        flexGrow: 1,
    },
    title: {
        marginRight: theme.spacing(2),
    },
}));

function App() {
    const classes = useStyles();
    const auth = useAuth();
    const navigate = useNavigate();

    const onLogOut = () => {
        auth.logOut();
        navigate("/login");
    };
    
    return (
        <div className={classes.root}>
            <AppBar position="static">
                <Toolbar>
                    <Typography variant="h6" className={classes.title}>
                        PSM Admin
                    </Typography>
                    <div className={classes.rightToolbar}>
                        <Button variant="h4" color="inherit" component={Link} to="/">
                            Home
                        </Button>
                        <Button variant="h4" color="inherit" component={Link} to="/users">
                            All users
                        </Button>
                        <Button variant="h4" color="inherit" component={Link} to="/tracks/search">
                            Tracks
                        </Button>
                        <Button variant="h" color="inherit" component={Link} to="authors/search">
                            Authors
                        </Button>
                    </div>
                    {auth.isLoaded &&
                        (auth.user ? (
                            <>
                                <Button color="inherit">
                                    {auth.user.userName}
                                </Button>
                                <Button color="inherit" onClick={onLogOut}>
                                    Log out
                                </Button>
                                <Button color="inherit" component={Link} to="/registration">
                                    Registration
                                </Button>
                            </>
                        ) : (
                            <>
                                <Button color="inherit" component={Link} to="/login">
                                    Login
                                </Button>
                            </>
                        ))}
                </Toolbar>
            </AppBar>
            <Routes/>
        </div>
    );
}

export default App;
