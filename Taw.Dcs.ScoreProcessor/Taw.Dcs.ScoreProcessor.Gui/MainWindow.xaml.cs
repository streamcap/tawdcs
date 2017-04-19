using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string _dateFormat;
        private readonly IDictionary<string, GameNameEntity> _games;
        private readonly IWindsorContainer _container;

        public MainWindow()
        {
            InitializeComponent();
            _dateFormat = ConfigurationManager.AppSettings["DateFormat"];
            if (ConfigurationManager.AppSettings["ColumnSeparator"].Length != 1)
            {
                throw new ConfigurationErrorsException("The ColumnSeparator app setting must be a single character!");
            }
            _container = new WindsorContainer().Install(new EngineInstaller(), new RepositoryInstaller(), new GuiLoggerInstaller());
            var logger = _container.Resolve<ILogger>();
            logger.Verbose("Starting up...");
            var repo = _container.Resolve<ITableStorageReadRepository>();

            _games = repo.QueryGameNames().OrderBy(g => g.RunTime).ToDictionary(GetGameNameText());
        }

        private Func<GameNameEntity, string> GetGameNameText()
        {
            return e => $"{e.GameName} ({e.RunTime.ToString(_dateFormat)})";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameNames.ItemsSource = _games;
            GameNames.SelectionChanged += GameNamesOnSelectionChanged;
            DuplicateAlgorithms.ItemsSource = ScoreAdjuster.GetDuplicateAlgorithmsList();
            DuplicateAlgorithms.SelectedIndex = 0;
        }

        private void GameNamesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearAllLists();
            var selectedGameName = (KeyValuePair<string, GameNameEntity>)GameNames.SelectedItem;
            var selectedGame = selectedGameName.Value;
            var selectedAlgorithm = (KillDuplicateAlgorithm)DuplicateAlgorithms.SelectedItem;

            var handler = _container.Resolve<IScoreEventsHandler>();
            var events = handler.GetCleanedScoreEvents(selectedGame, _dateFormat, false, selectedAlgorithm);

            PopulateTabs(events);
        }

        private void PopulateTabs(IList<ScoreEvent> events)
        {
            var summary = ScoreInformationTextBuilder.GetTeamsSummaryLines(events);
            foreach (var line in summary)
            {
                SummaryList.Items.Add(line);
            }
            var blueSide = ScoreInformationTextBuilder.GetTeamTopListLines(events, Constants.BlueCoalitionName, true, true);
            foreach (var line in blueSide)
            {
                BlueList.Items.Add(line);
            }
            var redSide = ScoreInformationTextBuilder.GetTeamTopListLines(events, Constants.RedCoalitionName, true, true);
            foreach (var line in redSide)
            {
                RedList.Items.Add(line);
            }
            var shame = ScoreInformationTextBuilder.GetPenaltiesLines(events, Constants.AllCoalitions);
            foreach (var line in shame)
            {
                WallOfShame.Items.Add(line);
            }
            Tabs.SelectedIndex = 0;
        }

        private void ClearAllLists()
        {
            SummaryList.Items.Clear();
            BlueList.Items.Clear();
            RedList.Items.Clear();
            WallOfShame.Items.Clear();
        }
    }

    public class GuiLoggerInstaller : LoggerInstaller
    {
        protected override ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.File(ConfigurationManager.AppSettings["LogFilePath"])
                .MinimumLevel.Verbose()
                .CreateLogger();
        }
    }
}
