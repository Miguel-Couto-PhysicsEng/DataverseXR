# DataVerseXR

**DataVerseXR** is an **Augmented/Virtual Reality** application developed during my 2025 summer internship at INESC TEC, built with **Unity 6.1**, **XCharts**, and the **Meta All-in-One SDK**, for the **Meta Quest 3** platform. This tool enables interactive 3D data visualization from CSV files through immersive AR charts.  

## Purpose

To allow users to visually explore complex datasets in immersive environments, making it easier to analyze, compare, and understand patterns through three-dimensional charts.  

## Features

- Import CSV files with a customizable structure  
- Automatic generation of the following chart types:  
  - **Line Charts** (Basic, Smooth, Dashed, Step, Log, Smooth Area, Stack Line, Time Line)  
  - **Bar Charts** (Basic Bar, Percent Column, Stacked Column, Zebra Column)  
  - **Pie Charts** (Basic, Donut, Radius Rose)  
  - **Radar Charts** (Circle, Polygon)  
  - **Scatter Charts** (Basic, Bubble)  
  - **Polar Charts** (Polar Line)  
  - **Candlestick Charts**  
  - **Ring Charts**  
- Interactive menus to select files and chart types  
- Tooltips with dynamic coordinates in AR mode  
- Full compatibility with Meta Quest 3 interactions (ray/poke/touch)  

## Demo Video
[![Watch the demo](docs/screenshots/menu.png)]([https://drive.google.com/file/d/1pRJVKFX-BMJ3TJjhTwCZxWppejEEoyHm/view](https://drive.google.com/file/d/1pRJVKFX-BMJ3TjJhTwCZxWppejEEoyHm/view?usp=sharing))


## CSV File Structure  

Example for a **Candlestick Chart**:  

```csv
X;Open;Close;Low;High
0;100;105;98;110
1;105;102;101;108
...

