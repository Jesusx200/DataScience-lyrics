<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sentiment.aspx.cs" Inherits="WebDS.Sentiment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analisis de Sentimientos</title>

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <!--JQuery-->
    <script type="text/javascript" src="/Scripts/jquery-3.4.1.min.js"></script>
    <!-- Bootstrap core CSS -->
    <link rel="stylesheet" href="/Content/bootstrap.min.css" />
    <script type="text/javascript" src="/Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="/Scripts/modernizr-2.8.3.js"></script>
    <!-- D3.js -->
    <script type="text/javascript" src="/Scripts/d3.v7.js"></script>
    <script type="text/javascript" src="/Scripts/d3.layout.cloud.js"></script>

    <script type="text/javascript">

        function enableRank() {
            let cmb = $('#DropDownYear').val();

            if (cmb != "Seleccione una opcion") {
                $('#DropDownRank').attr('disabled', false);
            } else {
                $('#DropDownRank').attr('disabled', true);
                $('#DropDownRank').val("Todo");
            }
            $('#ButtonLyric').hide();

            return false;
        }

        function enableLyric() {
            let cmbSent = $('#DropDownSentiment').val();
            if (cmbSent > 1) {
                $('#ButtonLyric').hide();
            }
        }

        function validateShowBar() {
            let cmbYear = $('#DropDownYear').val();

            if (cmbYear != "Seleccione una opcion") {
                let cmbRank = $('#DropDownRank').val();
                let cmbSent = $('#DropDownSentiment').val();

                $.ajax({
                    type: 'POST',
                    url: 'Sentiment.aspx/GetBar',
                    async: true,
                    data: '{ year:"' + cmbYear + '", rank:"' + cmbRank + '", senti:"' + cmbSent + '"}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: dataBar
                });
            }
            return false;
        }

        function dataBar(data) {
            let mydata = data.d;
            let cmbYear = $('#DropDownYear').val();
            let cmbRank = $('#DropDownRank').val();
            let cmbSent = $('#DropDownSentiment').val();
            let width = 800;

            $('#DropDownLast').val(cmbRank);
            $('#ButtonLyric').hide();
            $('#labelAuthor').hide();
            resetProgressCloud();
            resetTableDataCloud();

            $('#viewDiv').removeClass('horizontal scrollable');
            $('#viewDiv').addClass('row');

            if (mydata.length > 0) {
                if (cmbYear == 'Todo') {
                    width = 4900;
                    $('#viewDiv').removeClass('row');
                    $('#viewDiv').addClass('horizontal scrollable');
                }
                showBar(mydata, width);
                dataPie(mydata);

                $('#infoPos').show();
                $('#infoNeu').show();
                $('#infoNeg').show();

                if (mydata.length == 1) {
                    if (cmbSent == '1') {
                        $('#ButtonLyric').show();
                    }
                    loadAuthor(cmbYear, cmbRank);
                }

            } else {
                alert('No data');
            }
            return false;
        }

        function dataPie(mydata) {
            let sums = [0, 0, 0];
            for (let i = 0; i < mydata.length; i++) {
                let single = mydata[i].values;
                sums[0] += single[0];
                sums[1] += single[1];
                sums[2] += single[2];
            }
            let cneg = sums[0];
            let cneu = sums[1];
            let cpos = sums[2];
            $('#pieLabels').hide();

            let total = sums[0] + sums[1] + sums[2];
            sums[0] = Math.round(sums[0] / total * 100);
            sums[1] = Math.round(sums[1] / total * 100);
            sums[2] = Math.round(sums[2] / total * 100);

            let data = [
                { label: 'Negative', value: sums[0] },
                { label: 'Neutral', value: sums[1] },
                { label: 'Positive', value: sums[2] }
            ];
            showPie(data);
            $('#viewPie').show();
            if (mydata.length > 1) {
                showPieLabels(cpos, cneu, cneg);
            }

            return false;
        }

        function showBar(data, wid) {
            d3.selectAll("#myd3 > *").remove();
            $("#myd3").height("500px");

            const svg = d3.select("#myd3");
            const margin = { top: 20, right: 20, bottom: 20, left: 20 };
            const width = wid;
            const height = 500;

            const xScale = d3.scaleBand()
                .domain(data.map(d => d.category))
                .range([margin.left, width - margin.right])
                .padding(0.4);

            const yScale = d3.scaleLinear()
                .domain([0, d3.max(data, d => d3.max(d.values))])
                .range([height - margin.bottom, margin.top]);

            const colorScale = d3.scaleOrdinal()
                .domain(data.map(d => d.category))
                .range(["green", "red", "gray"]);

            const group = svg.selectAll(".group")
                .data(data)
                .enter()
                .append("g")
                .attr("class", "group")
                .attr("transform", d => `translate(${xScale(d.category)}, 0)`);

            const bars = group.selectAll(".bar")
                .data(d => d.values)
                .enter()
                .append("rect")
                .attr("class", "bar")
                .attr("x", (d, i) => i * (xScale.bandwidth() / 3))
                .attr("y", height - margin.bottom)
                .attr("width", xScale.bandwidth() / 3)
                .attr("height", 0)
                .attr("fill", (d, i) => colorScale(i));

            bars.transition()
                .duration(1000)
                .delay((d, i) => i * 100)
                .attr("y", d => yScale(d))
                .attr("height", d => height - yScale(d) - margin.bottom);

            svg.selectAll('.label')
                .data(data)
                .enter()
                .append('rect')
                .attr('class', 'label-bg')
                .attr('x', d => xScale(d.category) + xScale.bandwidth() / 2 - 20)
                .attr('y', height - 30)
                .attr('width', 40)
                .attr('height', 20)
                .attr('fill', 'white');

            svg.selectAll('.label-text')
                .data(data)
                .enter()
                .append('text')
                .attr('class', 'label-text')
                .attr('x', d => xScale(d.category) + xScale.bandwidth() / 2)
                .attr('y', height - 15)
                .attr('text-anchor', 'middle')
                .text(d => d.category);

            return false;
        }

        function showPie(data) {
            const width = 200;
            const height = 200;
            const radius = Math.min(width, height) / 2;

            d3.selectAll('#myd3Pie > *').remove();
            const svg = d3.select('#myd3Pie')
                .append('svg')
                .attr('width', width)
                .attr('height', height)
                .append('g')
                .attr('transform', `translate(${width / 2}, ${height / 2})`);

            const color = d3.scaleOrdinal()
                .domain(data.map(d => d.label))
                .range(["red", "gray", "green"]);

            const pie = d3.pie().value(d => d.value);
            const arcs = pie(data);

            const arcGenerator = d3.arc()
                .innerRadius(0)
                .outerRadius(radius);

            svg.selectAll('path')
                .data(arcs)
                .enter()
                .append('path')
                .attr('d', arcGenerator)
                .attr('fill', d => color(d.data.label))
                .attr('stroke', 'black')
                .style('stroke-width', '2px')
                .style('opacity', 0)
                .transition()
                .duration(1000)
                .style('opacity', 0.7)
                .each(function (d) {
                    d3.select(this)
                        .append('title')
                        .text(`${d.data.label}: ${d.data.value}%`);
                });

            svg.selectAll('text')
                .data(arcs)
                .enter()
                .append('text')
                .attr('transform', d => `translate(${arcGenerator.centroid(d)})`)
                .attr('dy', '0.35em')
                .attr('text-anchor', 'middle')
                .style('opacity', 0)
                .transition()
                .duration(1000)
                .style('opacity', 1)
                .text(d => d.data.value + "%");

            return false;
        }

        function showPieLabels(positive, neutral, negative) {
            $('#LabelPiePos').text("Positivo : " + positive);
            $('#LabelPieNeu').text("Neutral  : " + neutral);
            $('#LabelPieNeg').text("Negativo : " + negative);

            $('#pieLabels').show();
        }

        function validateLyrics() {
            let cmbYear = $('#DropDownYear').val();

            if (cmbYear != "Seleccione una opcion") {
                let cmbRank = $('#DropDownRank').val();

                $.ajax({
                    type: 'POST',
                    url: 'Sentiment.aspx/GetLyrics',
                    async: true,
                    data: '{ year:"' + cmbYear + '", rank:"' + cmbRank + '"}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: dataLyrics
                });
            }
            return false;
        }

        function dataLyrics(data) {
            let myLyricData = data.d;
            resetWords();

            if (myLyricData.length > 0) {
                colorWords(myLyricData);

                $('#lyricModal').modal('show');
            } else {
                alert('No data');
            }
            return false;
        }

        function DropDownChange() {
            let cmbRank = $('#DropDownRank').val();
            let last = $('#DropDownLast').val();

            if (cmbRank === last) { return false; }
            $('#ButtonLyric').hide();
            return false;
        }

        function colorWords(wordData) {
            // 0:neutral, 1:negative, 2:positive
            const wordListContainer = $('#textColor');

            $.each(wordData, function (index, item) {
                if (item.word === '|') {
                    wordListContainer.append('<br/>');
                } else {
                    const span = $('<span>').text(item.word + " ");
                    span.attr('data-value', item.value);
                    wordListContainer.append(span);
                }
            });

            $('#textColor span').each(function () {
                const value = parseFloat($(this).data('value'));
                let color;

                if (value == 1) {
                    color = 'red';
                } else if (value == 2) {
                    color = 'green';
                } else {
                    color = 'black';
                }

                $(this).css('color', color);
                if (color !== 'black') {
                    $(this).css('font-weight', 'bold');
                }
            });

            return false;
        }

        function resetWords() {
            $('#textColor').empty();

            return false;
        }

        function loadAuthor(cmbYear, cmbRank ) {
            $.ajax({
                type: 'POST',
                url: 'Sentiment.aspx/GetAuthor',
                async: true,
                data: '{ year:"' + cmbYear + '", rank:"' + cmbRank + '"}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: showAuthor
            });

            return false;
        }

        function showAuthor(data) {
            let author = data.d;
            $('#labelAuthor').text(author);
            $('#labelAuthor').show(800);

            return false;
        }

        function loadCloud() {
            let cmbYear = $('#DropDownYear').val();

            if (cmbYear != "Seleccione una opcion") {
                let cmbYear = $('#DropDownYear').val();
                let cmbRank = $('#DropDownRank').val();

                hideOther();
                $('#viewDiv').removeClass('horizontal scrollable');
                $('#viewDiv').addClass('row');

                resetTableDataCloud();
                resetProgressCloud();
                $('#progressCloud').show();

                $.ajax({
                    type: 'POST',
                    url: 'Sentiment.aspx/GetCloud',
                    async: true,
                    data: '{ year:"' + cmbYear + '", rank:"' + cmbRank + '"}',
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    xhr: function () { // Progress Bar
                        var xhr = new window.XMLHttpRequest();
                        xhr.addEventListener("progress", function (e) {
                            if (e.lengthComputable) {
                                var percentComplete = (e.loaded / e.total) * 100;
                                $("#progressLoad").text(percentComplete.toFixed(2) + "%");
                                $("#progressLoad").css("width", percentComplete + "%");
                                if (percentComplete == 100) {
                                    $('#progressCloud').hide(5000);
                                }
                            }
                        });
                        return xhr;
                    },
                    success: dataCloud
                });
            }
            return false;
        }

        function dataCloud(data) {
            let myCloudData = data.d;
            let cmbYearIndx = $("#DropDownYear").prop('selectedIndex');
            let cmbRankIndx = $('#DropDownRank').prop('selectedIndex');

            if (myCloudData.length > 0) {
                showCloud(myCloudData);
                loadTableDataCloud(myCloudData);

                if (cmbYearIndx > 1 & cmbRankIndx > 0) {
                    let cmbYear = $('#DropDownYear').val();
                    let cmbRank = $('#DropDownRank').val();

                    loadAuthor(cmbYear, cmbRank);
                    let cmbSent = $('#DropDownSentiment').val();
                    if (cmbSent == 1) {
                        $('#ButtonLyric').show();
                    }
                }

            } else {
                resetProgressCloud();
                resetTableDataCloud();
                $('#ButtonLyric').hide();
                alert('No data');
            }
            return false;
        }

        function showCloud(data) {
            d3.selectAll("#myd3 > *").remove();
            $("#myd3").height("600px");
            
            const svg = d3.select("#myd3");
            const margin = { top: 20, right: 20, bottom: 20, left: 20 };
            const width = 800; // 500
            const height = 600; // 480

            const layout = d3.layout.cloud()
                .size([width, height])
                .words(data.map(d => ({ text: d.word, size: d.count })))
                .padding(5)
                //.rotate(() => ~~(Math.random() * 2) * 90)
                .rotate(function () { return 0; })
                .font('Impact')
                .fontSize(d => d.size)
                .on('end', function (words) {
                    svg.append('g')
                        .attr('transform', `translate(${width / 2},${height / 2})`)
                        .selectAll('text')
                        .data(words)
                        .enter().append('text')
                        .style('font-size', d => `${d.size}px`)
                        .attr('opacity', 0)
                        .style('font-family', 'Impact')
                        .style('fill', (_, i) => d3.schemeCategory10[i % 20])
                        .attr('text-anchor', 'middle')
                        .attr('transform', d => {
                            if (Math.abs(d.rotate) !== 90) {
                                return `translate(-100,${d.y})rotate(${d.rotate})`;
                            } else {
                                return `translate(${d.x},-100)rotate(${d.rotate})`;
                            }
                        })
                        .text(d => d.text)
                        .each(function (_, i) {
                            d3.select(this)
                                .transition()
                                .delay(i * 100)
                                .duration(50)
                                .attr('opacity', 1)
                                .delay(i * 100)
                                .duration(100)
                                .attr('transform', d => `translate(${d.x},${d.y})rotate(${d.rotate})`);
                        });
                });
            layout.start();

            return false;
        }

        function hideOther() {
            $('#infoPos').hide();
            $('#infoNeu').hide();
            $('#infoNeg').hide();

            $('#labelAuthor').hide();

            $('#viewPie').hide();
            d3.selectAll('#myd3Pie > *').remove();
            d3.selectAll("#myd3 > *").remove();
            return false;
        }

        function resetProgressCloud() {
            $('#progressCloud').hide();
            $("#progressLoad").text("0%");
            $("#progressLoad").css("width", "0%");
            return false;
        }

        function loadTableDataCloud(data) {
            var table = $("#tableCloud");

            for (let i = 0; i < data.length; i++) {
                var newRow = "<tr><td>" + (i + 1) + "</td><td>" + data[i].real + "</td><td>"
                    + data[i].word + "</td></tr>";
                table.append(newRow);
            }
            $('#viewTable').show();
            $('#LabelFrec').show();

            return false;
        }

        function resetTableDataCloud() {
            $('#viewTable').hide();
            $('#LabelFrec').hide();
            let table = $('#tableCloud tbody');
            table.empty();
            var newRow = "<tr><th>Nro.</td><th>Cant.</td><th>Palabra</td></tr>";
            table.append(newRow);

            return false;
        }

    </script>
</head>

<body>
    <div class="container">
        <form id="form1" runat="server">

            <div class="row">
                <div class="col-12">
                    <h2>Opciones</h2>
                </div>
            </div>

            <div class="row">
                <div class="col-4">
                    <div class="form-group row mt-3">
                        <div class="col-sm-3 col-form-label">
                            <asp:Label ID="LabelYear" runat="server" Text="Year"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DropDownYear" CssClass="form-select" onchange="return enableRank();"
                                runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group row mt-3">
                        <div class="col-sm-3 col-form-label">
                            <asp:Label ID="LabelRank" runat="server" Text="Ranking"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DropDownRank" CssClass="form-select" runat="server" Enabled="False"
                                onblur="return DropDownChange();">
                            </asp:DropDownList>
                            <asp:HiddenField ID="DropDownLast" runat="server" />
                        </div>
                    </div>
                    <div class="form-group row mt-3">
                        <div class="col-sm-3 col-form-label">
                            <asp:Label ID="LabelAnalyze" runat="server" Text="Analizador"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DropDownSentiment" CssClass="form-select" onchange="return enableLyric();"
                                runat="server"></asp:DropDownList>
                        </div>
                    </div>
                    <!-- Buttons -->
                    <div class="text-center mt-3">
                        <div class="btn-group">
                            <asp:Button ID="ButtonBar" runat="server" Text="Analizar" class="btn btn-success"
                                OnClientClick="return validateShowBar();" />
                            <asp:Button ID="ButtonCloud" runat="server" Text="Word Cloud" class="btn btn-primary"
                                OnClientClick="return loadCloud();" />
                        </div>
                    </div>

                    <div class="text-center mt-3">
                        <div class="btn-group">
                            <asp:Button ID="ButtonLyric" runat="server" Text="Análisis de Letra" class="btn btn-warning"
                                OnClientClick="return validateLyrics();" style="display: none;"/>
                        </div>
                    </div>
                    <div class="row" id="LabelFrec" style="display: none;">
                        <div class="col-3">
                            <h5>Frecuencias</h5>
                        </div>
                    </div>
                    <div id="viewTable" class="row" style="display: none;">
                        <div class="col-10">
                             <div class="border" style="height: 250px; overflow-y: auto;">
                                <asp:Table id="tableCloud" class="table table-hover table-sm" runat="server">
                                    <asp:TableHeaderRow>
                                        <asp:TableHeaderCell>Nro.</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Cant.</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Palabra</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                </asp:Table>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-8">
                    <div class="row">
                        <div class="col-4">
                            <div class="row">
                                <div id="infoPos" class="badge text-bg-success text-wrap" style="display: none;">Positivo</div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="row">
                                <div id="infoNeu" class="badge text-bg-secondary text-wrap" style="display: none;">Neutral</div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="row">
                                <div id="infoNeg" class="badge text-bg-danger text-wrap" style="display: none;">Negativo</div>
                            </div>
                        </div>
                    </div>
                    <div id="viewDiv" class="row">
                        <svg id="myd3" style="width: 5000px; height: 500px"></svg>
                    </div>
                    <!-- Progress Bar -->
                    <div id="progressCloud" class="progress" role="progressbar" aria-valuemin="0" aria-valuemax="100" style="display: none;">
                        <div id="progressLoad" class="progress-bar" style="width: 0%">0%</div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <div id="labelAuthor" class="alert alert-primary text-center" style="display: none;" role="alert">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="viewPie" class="row mt-5" style="display: none;">
                <div class="row justify-content-center">    
                    <div class="col-3">
                    
                        <div class="col-md-auto">
                            <svg id="myd3Pie" style="width: 280px; height: 250px"></svg>
                        </div>
                    </div>
                
                    <div id="pieLabels" class="col-2" style="display: none;">
                         <div class="form-group row mt-5">
                            <div class="col-form-label">
                                <asp:Label ID="LabelPiePos" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-form-label">
                                <asp:Label ID="LabelPieNeu" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-form-label">
                                <asp:Label ID="LabelPieNeg" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="lyricModal" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5">Lyrics</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                     <div class="row">
                         <div class="col-4">
                             <div class="row">
                                 <div class="badge text-bg-success text-wrap">Positivo</div>
                             </div>
                         </div>
                         <div class="col-4">
                             <div class="row">
                                 <div class="badge text-bg-secondary text-wrap">Neutral</div>
                             </div>
                         </div>
                         <div class="col-4">
                             <div class="row">
                                 <div class="badge text-bg-danger text-wrap">Negativo</div>
                             </div>
                         </div>
                     </div>
                    <div class="row mt-3">
                        <div id="textColor" class="text-center"></div>
                    </div>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal" 
                        onclick="return resetWords();">Close</button>
                </div>
            </div>
        </div>
    </div>

</body>
</html>
