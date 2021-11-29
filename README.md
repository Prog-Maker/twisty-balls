# About

Simple celestial body simulation demo. Main purpose is to demonstrate [LeoQuery](https://github.com/kkolyan/leoquery) 
extension for [LeoECS Lite](https://github.com/Leopotam/ecslite). But it also now contains pure LeoECS Lite version, 
so could be used for comparison.

**Esc** to quit, **F5** to restart.

# Versions
To investigate performance overhead of LeoQuery there are a number of versions. They has equivalent behavior and the same 
game config. All mentioned standalones are built with IL2CPP.

### Straight forward
All implemented in most simple way with LeoQuery.
* [Profiler screenshot](perf/profiler_deopt.png)
* [standalone screenshot (27.67 fps)](perf/stand5000_deopti.png)
* Code branch: `master` ([link](../../tree/master))
* Download: [Windows x86_64](https://drive.google.com/file/d/1w2QMvpyUOdN3CUsnGQQDA3PfdwuVk5II/view?usp=sharing)

### Hot-spots in pure LeoECS Lite
Hot places rewritten using low-level LeoECS Lite API.
* [Profiler screenshot](perf/profiler_opt.png)
* [Game screenshot (35.05 fps)](perf/stand5000_opti.png)
* Code branch: `lite_opt` ([link](../../tree/lite_opt))
* Download: [Windows x86_64](https://drive.google.com/file/d/1BjHGHIPlCEtSkbzr-SUH5h78KfUByXnO/view?usp=sharing)

### Pure LeoECS Lite
Only LeoECS Lite with official DI extension is used
* [Profiler screenshot](perf/profiler_lit.png)
* Code branch: `pure_lite` ([link](../../tree/pure_lite))
* [Standalone screenshot (36.67 fps)](perf/stand5000_liti.png)
* Download: [Windows x86_64](https://drive.google.com/file/d/1Id9BVIL7g0Sk-13XnbLImiFZgAzDU-wm/view?usp=sharing)
