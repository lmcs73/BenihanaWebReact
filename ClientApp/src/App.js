import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { RunSimulation } from "./components/RunSimulation";
import * as signalR from "@microsoft/signalr";

import "./custom.css";

export default class App extends Component {
  static displayName = App.name;

  componentDidMount() {
    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/chatHub")
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.setState({ hubConnection }, () => {
      this.state.hubConnection
        .start()
        .then(() => {
          console.log("connected");
          this.state.hubConnection.invoke(
            "sendConnectionId",
            hubConnection.connectionId
          );
        })
        .catch(() => console.log("Error while establishing connection"));
      this.state.hubConnection.on("setClientMessage", data => {
        console.log(data);
      });
    });
  }

  render() {
    return (
      <Layout>
        <Route exact path="/" component={Home} />
        <Route path="/run/:id" component={RunSimulation} />
      </Layout>
    );
  }
}
