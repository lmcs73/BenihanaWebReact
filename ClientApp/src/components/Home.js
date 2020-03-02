import React, { useState, useEffect } from "react";
import { Configuration } from "./Configuration.js";
import { LoadingDots } from "./LoadingDots";
import ResultTable from "./ResultTable";
import { RunSimulation } from "./RunSimulation";
import axios from "axios";

export const Home = () => {
  const [simulationResult, setSimulationResult] = useState([]);
  const [loading, setLoading] = useState(false);
  const [playSimulationResult, setPlaySimulationResult] = useState(null);

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
    console.log(simulationResult[index]);
    setPlaySimulationResult(simulationResult[index]);
    console.log(`Play simulation ${simulationResult[index]}`);
  };

  const handleCloseSimulation = () => {
    setPlaySimulationResult(null);
  };

  useEffect(() => {
    setSimulationResult(
      localStorage.getItem("results") !== null
        ? JSON.parse(localStorage.getItem("results"))
        : []
    );
  }, []);

  let body;

  if (playSimulationResult !== null) {
    body = (
      <RunSimulation
        result={playSimulationResult}
        onClose={handleCloseSimulation}
      />
    );
  } else {
    body = (
      <div>
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
  }
  return (
    <div>
      <h1 className="text-center">Restaurant Simulation Web</h1>
      {body}
    </div>
  );
};
