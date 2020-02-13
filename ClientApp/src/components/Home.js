import React, { useState, useEffect } from "react";
import { Configuration } from "./Configuration.js";
import { LoadingDots } from "./LoadingDots";
import ResultTable  from "./ResultTable";
import axios from "axios";

export const Home = () => {
  const [simulationResult, setSimulationResult] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleRunSimulation = async config => {
    console.log(config);
    setLoading(true);
    const response = await axios.post("api/simulation", config, {
      headers: { "Content-Type": "application/json" }
    });
    console.log(response.data);
    setSimulationResult(response.data);
    localStorage.setItem("results", JSON.stringify(response.data));
    localStorage.setItem("config", JSON.stringify(config));
    setLoading(false);
  };

  const handlePlaySimulation = index => {
    console.log(`Clicked on index ${index}`);
  };

  useEffect(() => {
    setSimulationResult(
      localStorage.getItem("results") !== null
        ? JSON.parse(localStorage.getItem("results"))
        : []
    );
  }, []);

  return (
    <div>
      <h1 className="text-center">Restaurant Simulation Web!</h1>
      <Configuration onSubmitRun={handleRunSimulation} />
      {loading ? (
        <LoadingDots />
      ) : (
        <div>
          <ResultTable
            results={simulationResult}
            onPlaySimulation={handlePlaySimulation}
          />
        </div>
      )}
    </div>
  );
};
