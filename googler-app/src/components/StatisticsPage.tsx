import { Box, Button, Container, Grid, Input, Typography } from "@mui/material";
import React, { useState } from "react";
import { StatisticsApi } from "../services/googler-api/api";
import { Statistics } from "../services/googler-api";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { response } from "express";
import { AxiosResponse } from "axios";

const ariaLabel = { 'aria-label': 'description' };

const statisticsApi = new StatisticsApi({ basePath: process.env.REACT_APP_GOOGLER_API_BASE_PATH });

const StatisticsPage: React.FunctionComponent<{}> = () => {
    const [keywords, setKeywords] = useState("");
    const [website, setWebsite] = useState("infotrack");
    const [numResults, setNumResults] = useState(100);
    const [searching, setSearching] = useState(false);
    const [statistics, setStatistics] = useState<Statistics | undefined>(undefined);

    const handleSubmit = async () => {
        statisticsApi.getStatistics(keywords, website)
            .then((response: AxiosResponse<Statistics, any>) => {
                setStatistics(response.data);
            }).catch((error: any) => {
                toast.error(`Error getting statistics from api. Message: ${error}`);
            }
        );
    }

    return (
        <Container maxWidth="sm">
            <Box sx={{ my: 4 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    InfoTrack Google Statisticss
                </Typography>
                <Typography variant="h5" gutterBottom>
                    Search google for some awesome stuff
                </Typography>
                <Typography variant="h6" gutterBottom>
                    Enter some parameters and see what comes back!
                </Typography>
                <Typography variant="body1" gutterBottom>
                    Keywords
                </Typography>
                <Input
                    fullWidth
                    placeholder="Keywords to search for..."
                    inputProps={ariaLabel}
                    required
                    onChange={(e) => setKeywords(e.target.value)}
                />
                <Typography variant="body1" gutterBottom>
                    Google search results count
                </Typography>
                <Input
                    fullWidth
                    defaultValue="100"
                    inputProps={ariaLabel}
                    required
                    onChange={(e) => setNumResults(parseInt(e.target.value, 10))}
                />
                <Typography variant="body1" gutterBottom>
                    Website to quant
                </Typography>
                <Input
                    fullWidth
                    defaultValue="infotrack"
                    inputProps={ariaLabel}
                    required
                    onChange={(e) => setWebsite(e.target.value)}
                />
                <Button onClick={handleSubmit} variant="contained" type="submit" sx={{ marginTop: 1 }}>
                    Search
                </Button>
            </Box>
            <ToastContainer />
        </Container>
    );
}

export default StatisticsPage;