import {Routes, Route, Navigate, useParams} from "react-router-dom";
import Home from "../../pages/Home";
import Login from "../../pages/Login";
import Registration from "../../pages/Registration";
import NotFound from "../../pages/NotFound";
import useAuth from "../../hooks/useAuth";
import PrivateRoute from "../components/PrivateRoute";
import GuestRoute from "../components/GuestRoute";
import {
    CircularProgress,
    makeStyles,
    Container,
    Grid,
} from "@material-ui/core";
import All from "../../pages/Users/all";
import User from "../../pages/Users/user";
import TracksSearch from "../../pages/Tracks/Search";
import AuthorsSearch from "../../pages/Authors/Search";
import Author from "../../pages/Authors/Index";
import Index from "../../pages/Authors/Index";
import 'bootstrap/dist/css/bootstrap.min.css';

const useStyles = makeStyles((theme) => ({
    root: {
        padding: theme.spacing(3),
    },
}));

function AppRoutes() {
    const classes = useStyles();
    const auth = useAuth();

    const WrappedUser = function () {
        return <User id={useParams().id}/>;
    }

    const WrappedAuthor = function () {
        return <Author id={useParams().id}/>
    }

    return auth.isLoaded ? (
        <Routes>
            <Route path="/" element={<Home/>}/>
            <Route
                path="/login"
                element={
                    <GuestRoute>
                        <Login/>
                    </GuestRoute>
                }
            />
            <Route
                path="/registration"
                element={
                    <PrivateRoute>
                        <Registration/>
                    </PrivateRoute>
                }
            />
            <Route path="/users" element={<PrivateRoute><All/></PrivateRoute>}/>
            <Route path="/user/:id" element={<PrivateRoute><WrappedUser/></PrivateRoute>}/>
            <Route path="/tracks/search" element={<PrivateRoute><TracksSearch/></PrivateRoute>}/>
            <Route path="/authors/search" element={<PrivateRoute><AuthorsSearch/></PrivateRoute>}/>
            <Route path="/author/:id" element={<PrivateRoute><WrappedAuthor/></PrivateRoute>}/>

            <Route path="/not-found-404" element={<NotFound/>}/>
            <Route path="*" element={<Navigate to="/not-found-404"/>}/>
        </Routes>
    ) : (
        <Container maxWidth="md" className={classes.root}>
            <Grid container spacing={3} alignItems="center" justifyContent="center">
                <Grid item>
                    <CircularProgress color="inherit"/>
                </Grid>
            </Grid>
        </Container>
    );
}

export default AppRoutes;
