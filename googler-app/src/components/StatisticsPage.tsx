import { Box, Button, Container, Input, ListItem, ListItemText, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { FixedSizeList as List, ListChildComponentProps } from 'react-window';
import { SearchResultsApi } from "../services/googler-api/api";
import { SearchResult } from "../services/googler-api";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import AutoSizer from "react-virtualized-auto-sizer";
import { AxiosResponse } from "axios";

const ariaLabel = { 'aria-label': 'description' };

const searchResultsApi = new SearchResultsApi({ basePath: process.env.REACT_APP_GOOGLER_API_BASE_PATH });

interface Statistics {
    topTenCount: number;
    topTwentyFiveCount: number;
    totalCount: number;
}

const StatisticsPage: React.FunctionComponent<{}> = () => {
    const [keywords, setKeywords] = useState("");
    const [website, setWebsite] = useState("infotrack.com");
    const [numResults, setNumResults] = useState(100);
    const [fetching, setFetching] = useState(false);
    const [searchResults, setSearchResults] = useState<SearchResult[] | undefined>(undefined);
    const [statistics, setStatistics] = useState<Statistics | undefined>(undefined);

    useEffect(() => calculateStatistics(), [website, searchResults]);
    useEffect(() => setWebsite("infotrack.com"),[searchResults]);

    function rgbaToHexString(r: number, g: number, b: number, a: number) {
        let rs: string = r.toString(16);
        let gs: string = g.toString(16);
        let bs: string = b.toString(16);
        let as: string = Math.round(a * 255).toString(16);

        if (rs.length == 1)
            rs = "0" + r;
        if (gs.length == 1)
            gs = "0" + g;
        if (bs.length == 1)
            bs = "0" + b;
        if (as.length == 1)
            as = "0" + a;

        return "#" + rs + gs + bs + as;
    }

    function calculateStatistics() {
        if (website && website.length > 0 && searchResults) {
            let total = 0;
            let t25 = 0;
            let t10 = 0;
            for (let i = 0; i < searchResults.length; i++) {
                if (searchResults[i].domain?.includes(website)) {
                    if (i < 10) {
                        t10++;
                    }
                    if (i < 25) {
                        t25++;
                    }
                    total++;
                }
            }
            setStatistics({
                topTenCount: t10,
                topTwentyFiveCount: t25,
                totalCount: total
            });
        } else {
            setStatistics(undefined);
        }
    }

    async function handleSubmit(event: any) {
        event.preventDefault();
        setFetching(true);
        setStatistics(undefined);
        searchResultsApi.getSearchResults(keywords, numResults)
            .then((response: AxiosResponse<SearchResult[], any>) => {
                setSearchResults(response.data);
                setFetching(false);
            }).catch((error: any) => {
                toast.error(`Error getting statistics from api. Message: ${error}`);
                setFetching(false);
            });
    }

    function getBackgroundColor(index: number): import("csstype").Property.BackgroundColor | undefined {
        if (searchResults && index < searchResults.length) {
            const result: SearchResult = searchResults[index];
            if (website && result.domain?.includes(website)) {
                // this is a correct result, calculate the color
                const greenStrength = Math.trunc(((index / searchResults.length) * -255) + 255);
                const redStrength = Math.trunc(((index / searchResults.length) * 255));
                return rgbaToHexString(redStrength, greenStrength, 0, 0.75);
            }
        }
        return rgbaToHexString(255, 255, 255, 255);
    }

    return (
        <Container maxWidth="md">
            <Box sx={{ my: 6 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    InfoTrack Google Statistics
                </Typography>
                <Typography variant="h5" gutterBottom>
                    Search google for some awesome stuff
                </Typography>
                <Typography variant="h6" gutterBottom>
                    Enter some parameters and see what comes back!
                </Typography>
                <form onSubmit={handleSubmit}>
                    <div style={{ display: 'flex', paddingTop: 10, textAlign: "left", alignItems: "end" }}>
                        <Typography sx={{ flex: 2 }} variant="body1" gutterBottom>
                            Keywords
                        </Typography>
                        <Input
                            fullWidth
                            placeholder="Keywords to search for..."
                            inputProps={ariaLabel}
                            required
                            onChange={(e) => setKeywords(e.target.value)}
                            style={{ flex: 4 }}
                        />
                    </div>
                    <div style={{ display: 'flex', paddingTop: 10, textAlign: "left", alignItems: "end" }}>
                        <Typography sx={{ flex: 2 }} variant="body1" gutterBottom >
                            Google results count
                        </Typography>
                        <Input
                            fullWidth
                            defaultValue="100"
                            inputProps={ariaLabel}
                            required
                            onChange={(e) => setNumResults(parseInt(e.target.value, 10))}
                            style={{ flex: 4 }}
                        />
                    </div>
                    <Button disabled={fetching} variant="contained" type="submit" sx={{ marginTop: 1 }}>
                        {fetching ? "Searching..." : "Search"}
                    </Button>
                </form>
                {
                    searchResults ? (
                        <Box sx={{ width: '100%', height: 600, marginTop: 3 }}>
                            <Typography variant="h5" gutterBottom >
                                Results
                            </Typography>
                            <div style={{
                                display: "flex"
                            }}>
                                <Box
                                    sx={{ width: '100%', height: 600, bgcolor: 'background.paper' }}
                                >
                                    <AutoSizer>
                                        {({ height, width }) => (
                                            <List
                                                height={height}
                                                width={width}
                                                itemSize={40}
                                                itemCount={searchResults.length}
                                                overscanCount={5}
                                                style={{
                                                    listStyle: "none",
                                                }}
                                            >
                                                {({ data, index, style }) => {
                                                    return (
                                                        <li style={style}>
                                                            <div style={{
                                                                display: "flex",
                                                                justifyContent: "space-between",
                                                                textAlign: "left",
                                                                backgroundColor: getBackgroundColor(index),
                                                                padding: 5,
                                                                borderRadius: 5
                                                            }}>
                                                                <div style={{ flex: 1 }}>{index + 1}</div>
                                                                <div style={{ flex: 10 }}>{searchResults[index]?.domain}</div>
                                                            </div>
                                                        </li>
                                                    );
                                                }}
                                            </List>
                                        )}
                                    </AutoSizer>
                                </Box>
                                <Box
                                    sx={{ width: '100%', height: 600 }}
                                >
                                    <div style={{ display: 'flex', padding: 10, textAlign: "left", alignItems: "end" }}>
                                        <Typography sx={{ flex: 4, mx: 2 }} variant="body1" gutterBottom>
                                            Calculate statistics for...
                                        </Typography>
                                        <Input
                                            fullWidth
                                            inputProps={ariaLabel}
                                            required
                                            value={website}
                                            onChange={(e) => setWebsite(e.target.value)}
                                            style={{ flex: 4 }}
                                        />
                                    </div>
                                    {statistics ?
                                        <Box sx={{ width: '100%', height: 600 }}>
                                            <Typography variant="h6">
                                                Statistics
                                            </Typography>
                                            <Typography variant="subtitle1">
                                                Top 10 Count: {statistics.topTenCount}
                                            </Typography>
                                            <Typography variant="subtitle1">
                                                Top 25 Count: {statistics.topTwentyFiveCount}
                                            </Typography>
                                            <Typography variant="subtitle1">
                                                Total: {statistics.totalCount}
                                            </Typography>
                                        </Box> : <></>
                                    }
                                </Box>
                            </div>
                        </Box>
                    ) : <></>
                }
            </Box>
            <ToastContainer />
        </Container>
    );
}

export default StatisticsPage;