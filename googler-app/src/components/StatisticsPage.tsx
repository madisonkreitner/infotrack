import { Box, Button, Container, Input, ListItem, ListItemText, Typography } from "@mui/material";
import React, { useState } from "react";
import { FixedSizeList as List, ListChildComponentProps } from 'react-window';
import { SearchResultsApi } from "../services/googler-api/api";
import { SearchResult } from "../services/googler-api";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import AutoSizer from "react-virtualized-auto-sizer";
import { AxiosResponse } from "axios";

const ariaLabel = { 'aria-label': 'description' };

const searchResultsApi = new SearchResultsApi({ basePath: process.env.REACT_APP_GOOGLER_API_BASE_PATH });

const StatisticsPage: React.FunctionComponent<{}> = () => {
    const [keywords, setKeywords] = useState("");
    const [website, setWebsite] = useState("infotrack");
    const [numResults, setNumResults] = useState(100);
    const [searching, setSearching] = useState(false);
    const [fetching, setFetching] = useState(false);
    const [searchResults, setSearchResults] = useState<SearchResult[] | undefined>(undefined);

    const handleSubmit = async (event: any) => {
        event.preventDefault();
        setFetching(true);
        searchResultsApi.getSearchResults(keywords, numResults)
            .then((response: AxiosResponse<SearchResult[], any>) => {
                setSearchResults(response.data);
                setFetching(false);
            }).catch((error: any) => {
                toast.error(`Error getting statistics from api. Message: ${error}`);
                setFetching(false);
            }
            );
    }

    return (
        <Container maxWidth="sm">
            <Box sx={{ my: 6 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    InfoTrack Google Statisticss
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
                    <div style={{ display: 'flex', paddingTop: 10, textAlign: "left", alignItems: "end" }}>
                        <Typography sx={{ flex: 2 }} variant="body1" gutterBottom>
                            Website to quant
                        </Typography>
                        <Input
                            fullWidth
                            defaultValue="infotrack"
                            inputProps={ariaLabel}
                            required
                            onChange={(e) => setWebsite(e.target.value)}
                            style={{ flex: 4 }}
                        />
                    </div>
                    <Button disabled={fetching} variant="contained" type="submit" sx={{ marginTop: 1 }}>
                        {fetching ? "Searching..." : "Search"}
                    </Button>
                </form>
                {
                    searchResults ? (
                        <Box
                            sx={{ width: '100%', height: 600, bgcolor: 'background.paper', marginTop: 4 }}
                        >
                            <Typography variant="h5" gutterBottom >
                                Results
                            </Typography>
                            <AutoSizer>
                                {({ height, width }) => (
                                    <List
                                        height={height}
                                        width={width}
                                        itemSize={46}
                                        itemCount={searchResults.length}
                                        overscanCount={5}
                                        style={{
                                            listStyle: "none",
                                        }}
                                    >
                                        {({ data, index, style }) => {
                                            return (
                                                <li style={style}>
                                                    <div style={{display: "flex", justifyContent: "space-between", textAlign:"left"}}>
                                                        <div style={{flex: 1}}>{index + 1}</div>
                                                        <div style={{flex: 10}}>{searchResults[index]?.domain}</div>
                                                    </div>
                                                </li>
                                            );
                                        }}
                                    </List>
                                )}
                            </AutoSizer>
                        </Box>
                    ) : <></>
                }
            </Box>
            <ToastContainer />
        </Container>
    );
}

export default StatisticsPage;