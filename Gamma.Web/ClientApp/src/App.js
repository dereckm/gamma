import React, { Component } from 'react';
import { Route } from 'react-router';
import Home from './pages/Home.tsx';
import './App.css';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <div className='container'>
        <Route exact path='/' component={Home} />
      </div>
    );
  }
}
