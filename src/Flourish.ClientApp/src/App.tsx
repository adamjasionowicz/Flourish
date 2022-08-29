import React from "react";
import "./App.css";
import axios from "axios";

interface IWeatherForecast {
  dateFormatted: string;
  temperatureC?: number;
  temperatureF: number;
  summary: string;
}

const defaultWeatherForecasts: IWeatherForecast[] = [];

const App = () => {
  const [weatherForecasts, setWeatherForecasts]: [
    IWeatherForecast[],
    (weatherForecasts: IWeatherForecast[]) => void
  ] = React.useState(defaultWeatherForecasts);
  const [loading, setLoading]: [boolean, (loading: boolean) => void] =
    React.useState<boolean>(true);
  const [error, setError]: [string, (error: string) => void] =
    React.useState("");

  React.useEffect(() => {
    axios
      .get<IWeatherForecast[]>("api/WeatherForecast", {
        headers: {
          "Content-Type": "application/json",
        },
        timeout: 1500,
      })
      .then((response) => {
        setWeatherForecasts(response.data);
        setLoading(false);
      })
      .catch((ex) => {
        const error = axios.isCancel(ex)
          ? "Request Cancelled"
          : ex.code === "ECONNABORTED"
          ? "A timeout has occurred"
          : ex.response.status === 404
          ? "Resource not found"
          : "An unexpected error has occurred";
        setError(error);
        setLoading(false);
      });
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h3>Flourish React</h3>
        {loading && <b>Loading.</b>}
        <table className="table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Temp. (C)</th>
              <th>Temp. (F)</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {weatherForecasts.map((forecast, index) => (
              <tr key={forecast.dateFormatted}>
                <td>{forecast.dateFormatted}</td>
                <td>{forecast.temperatureC}</td>
                <td>{forecast.temperatureF}</td>
                <td>{forecast.summary}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {error && <b>{error}</b>}
      </header>
    </div>
  );
};

export default App;
